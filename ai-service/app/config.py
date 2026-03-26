import os

# Model faylının yolu
MODEL_PATH = os.getenv("FASTTEXT_MODEL_PATH", "data/cc.az.300.bin")

# Söz bankı faylı
WORD_BANK_PATH = os.getenv("WORD_BANK_PATH", "data/az_words.txt")

# Server
HOST = os.getenv("HOST", "0.0.0.0")
PORT = int(os.getenv("PORT", "8000"))
