FROM python:3.11-slim

WORKDIR /app

# Install native dependencies required for psycopg2 to compile
RUN apt-get update && apt-get install -y libpq-dev gcc

# Install Python packages
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy the new Clean Architecture folders and the entry point
COPY core/ ./core/
COPY infrastructure/ ./infrastructure/
COPY main.py .

# Run the new main script unbuffered
CMD ["python", "-u", "main.py"]