import logging
import psycopg2
from psycopg2.extensions import connection

from core.config import Settings as settings
from core.models import NodeTelemetry, NodeMessage

logger = logging.getLogger(__name__)

class OsintRepository:
    """Handles all database interactions with PostgreSQL/PostGIS."""
    
    def __init__(self) -> None:
        self._conn_str = (
            f"host={settings.DB_HOST} dbname={settings.DB_NAME} "
            f"user={settings.DB_USER} password={settings.DB_PASS}"
        )

    def _get_connection(self) -> connection:
        return psycopg2.connect(self._conn_str)

    def upsert_node_telemetry(self, telemetry: NodeTelemetry) -> None:
        """Upserts a node's battery, voltage, and spatial location."""
        if telemetry.latitude is None or telemetry.longitude is None:
            return  # Do not overwrite database with null GPS data

        query = """
            INSERT INTO meshtastic_nodes ("NodeId", "BatteryLevel", "Voltage", "LastHeard", "Location")
            VALUES (%s, %s, %s, CURRENT_TIMESTAMP, ST_SetSRID(ST_MakePoint(%s, %s), 4326))
            ON CONFLICT ("NodeId") DO UPDATE 
            SET "BatteryLevel" = EXCLUDED."BatteryLevel",
                "Voltage" = EXCLUDED."Voltage",
                "LastHeard" = CURRENT_TIMESTAMP,
                "Location" = EXCLUDED."Location";
        """
        try:
            with self._get_connection() as conn:
                with conn.cursor() as cur:
                    cur.execute(
                        query, 
                        (
                            telemetry.node_id, 
                            telemetry.battery_level, 
                            telemetry.voltage, 
                            telemetry.longitude, 
                            telemetry.latitude
                        )
                    )
            logger.info(f"📍 Upserted Node {telemetry.node_id} at {telemetry.latitude}, {telemetry.longitude}")
        except psycopg2.Error as e:
            logger.error(f"Database error upserting telemetry: {e}")

    def insert_message(self, message: NodeMessage) -> None:
        """Ensures the sender node exists, then inserts the mesh message."""
        ensure_node_query = """
            INSERT INTO meshtastic_nodes ("NodeId", "LastHeard") 
            VALUES (%s, CURRENT_TIMESTAMP) 
            ON CONFLICT ("NodeId") DO NOTHING;
        """
        insert_msg_query = """
            INSERT INTO meshtastic_messages ("SenderId", "ReceiverId", "Payload", "Snr", "Timestamp")
            VALUES (%s, %s, %s, %s, CURRENT_TIMESTAMP);
        """
        try:
            with self._get_connection() as conn:
                with conn.cursor() as cur:
                    # 1. Satisfy the C# Foreign Key constraint
                    cur.execute(ensure_node_query, (message.sender_id,))
                    # 2. Insert the actual payload
                    cur.execute(
                        insert_msg_query, 
                        (message.sender_id, message.receiver_id, message.payload, message.snr)
                    )
            logger.info(f"✉️ Saved message from {message.sender_id}")
        except psycopg2.Error as e:
            logger.error(f"Database error inserting message: {e}")