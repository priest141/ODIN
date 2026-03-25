import time
import logging
import meshtastic
import meshtastic.serial_interface
import meshtastic.tcp_interface
from pubsub import pub

from core.config import Settings as settings
from core.models import NodeTelemetry, NodeMessage
from infrastructure.database import OsintRepository

logger = logging.getLogger(__name__)

class MeshtasticAdapter:
    """Interfaces with the physical radio and maps events to the database."""
    
    def __init__(self, repository: OsintRepository) -> None:
        self.repository = repository
        self.interface = None

    def start(self) -> None:
        """Subscribes to events and opens the hardware connection."""
        pub.subscribe(self._on_position_receive, "meshtastic.receive.position")
        pub.subscribe(self._on_text_receive, "meshtastic.receive.text")
        
        self._connect()
        
        # Keep the main thread alive
        try:
            while True:
                time.sleep(1000)
        except KeyboardInterrupt:
            logger.info("Shutting down radio interface...")
            if self.interface:
                self.interface.close()

    def _connect(self) -> None:
        logger.info(f"Connecting to radio via {settings.RADIO_CONNECTION_TYPE}...")
        try:
            if settings.RADIO_CONNECTION_TYPE.lower() == "tcp":
                self.interface = meshtastic.tcp_interface.TCPInterface(hostname=settings.RADIO_IP)
            else:
                self.interface = meshtastic.serial_interface.SerialInterface(devPath=settings.RADIO_PORT)
            logger.info("✅ Connected! Listening for mesh traffic...")
        except Exception as e:
            logger.error(f"❌ Failed to connect to radio: {e}")
            raise

    def _on_position_receive(self, packet: dict, interface) -> None:
        """Callback triggered by the hardware when a GPS ping arrives."""
        try:
            sender_id = packet.get('fromId')
            decoded = packet.get('decoded', {})
            position = decoded.get('position', {})
            
            telemetry = NodeTelemetry(
                node_id=sender_id, # type: ignore
                battery_level=position.get('batteryLevel'),
                voltage=position.get('voltage'),
                latitude=position.get('latitude'),
                longitude=position.get('longitude')
            )
            
            self.repository.upsert_node_telemetry(telemetry)
        except Exception as e:
            logger.error(f"Error processing position packet: {e}")

    def _on_text_receive(self, packet: dict, interface) -> None:
        """Callback triggered by the hardware when a text message arrives."""
        try:
            decoded = packet.get('decoded', {})
            text = decoded.get('text')
            
            if not text:
                return

            message = NodeMessage(
                sender_id=packet.get('fromId'), # type: ignore 
                receiver_id=packet.get('toId', 'broadcast'),
                payload=text,
                snr=packet.get('rxSnr')
            )
            
            self.repository.insert_message(message)
        except Exception as e:
            logger.error(f"Error processing text packet: {e}")