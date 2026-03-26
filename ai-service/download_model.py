"""
fastText Azerbaijani model yükləyici.
Facebook-un pre-trained cc.az.300.bin modelini yükləyir.

İstifadə:
    python download_model.py
"""
import os
import urllib.request
import sys

MODEL_URL = "https://dl.fbaipublicfiles.com/fasttext/vectors-crawl/cc.az.300.bin.gz"
MODEL_DIR = "data"
MODEL_PATH = os.path.join(MODEL_DIR, "cc.az.300.bin")
GZ_PATH = MODEL_PATH + ".gz"


def download_model():
    os.makedirs(MODEL_DIR, exist_ok=True)

    if os.path.exists(MODEL_PATH):
        print(f"✅ Model artıq mövcuddur: {MODEL_PATH}")
        return

    print(f"📥 Model yüklənir: {MODEL_URL}")
    print("   Bu bir neçə dəqiqə çəkə bilər (~4GB)...")

    def progress_hook(block_num, block_size, total_size):
        downloaded = block_num * block_size
        percent = min(100, downloaded * 100 // total_size) if total_size > 0 else 0
        mb = downloaded / (1024 * 1024)
        total_mb = total_size / (1024 * 1024) if total_size > 0 else 0
        sys.stdout.write(f"\r   {percent}% ({mb:.0f}/{total_mb:.0f} MB)")
        sys.stdout.flush()

    urllib.request.urlretrieve(MODEL_URL, GZ_PATH, progress_hook)
    print("\n\n📦 Arxiv açılır...")

    import gzip
    import shutil
    with gzip.open(GZ_PATH, "rb") as f_in:
        with open(MODEL_PATH, "wb") as f_out:
            shutil.copyfileobj(f_in, f_out)

    os.remove(GZ_PATH)
    print(f"✅ Model hazırdır: {MODEL_PATH}")


if __name__ == "__main__":
    download_model()
