import os 
from dotenv import load_dotenv

load_dotenv()

class Settings:
    """Application configuration loaded from environment variables."""
    DB_HOST: str = os.getenv("DB_HOST", "localhost")
    DB_NAME: str = os.getenv("DB_NAME", "osint_dashboard")
    DB_USER: str = os.getenv("DB_USER", "postgres")
    DB_PASS: str = os.getenv("DB_PASS", "SuperSecretPassword123!")
    
    RADIO_CONNECTION_TYPE: str = os.getenv("RADIO_CONNECTION_TYPE", "serial")
    RADIO_PORT: str = os.getenv("RADIO_PORT", "/dev/ttyUSB0")
    RADIO_IP: str = os.getenv("RADIO_IP", "192.168.1.100")

