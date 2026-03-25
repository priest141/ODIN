import logging
from infrastructure.database import OsintRepository
from infrastructure.radio import MeshtasticAdapter

# Configure basic logging
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S"
)

def main() -> None:
    logging.info("Starting OSINT Meshtastic Worker...")
    
    # 1. Instantiate the database repository
    db_repository = OsintRepository()
    
    # 2. Inject the repository into the hardware adapter
    radio_adapter = MeshtasticAdapter(repository=db_repository)
    
    # 3. Start listening to the airwaves
    radio_adapter.start()

if __name__ == "__main__":
    main()