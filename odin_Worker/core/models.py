from dataclasses import dataclass
from typing import Optional

@dataclass
class NodeTelemetry:
    """Domain model representing physical node status and location."""
    node_id: str
    battery_level: Optional[int] = None
    voltage: Optional[float] = None
    latitude: Optional[float] = None
    longitude: Optional[float] = None

@dataclass
class NodeMessage:
    """Domain model representing a text payload sent over the mesh."""
    sender_id: str
    receiver_id: str
    payload: str
    snr: Optional[float] = None
