import fasttext
import numpy as np
from numpy.linalg import norm
import logging
import os
from app.config import MODEL_PATH

logger = logging.getLogger(__name__)


class FastTextService:
    """
    fastText modeli ilə söz vektorları arası cosine similarity hesablayır.
    Azerbaijani pre-trained model (cc.az.300.bin) istifadə edir.
    """

    def __init__(self):
        self.model = None
        self._word_vectors_cache: dict[str, np.ndarray] = {}

    def load_model(self):
        """Modeli yüklə. Əgər model faylı yoxdursa xəbərdarlıq ver."""
        if os.path.exists(MODEL_PATH):
            logger.info(f"Model yüklənir: {MODEL_PATH}")
            # Suppress fasttext warnings
            fasttext.FastText.eprint = lambda x: None
            self.model = fasttext.load_model(MODEL_PATH)
            logger.info(f"Model yükləndi. Vektor ölçüsü: {self.model.get_dimension()}")
        else:
            logger.warning(
                f"⚠️ Model faylı tapılmadı: {MODEL_PATH}. "
                f"Demo rejimində çalışacaq (random vektorlar)."
            )
            self.model = None

    def get_word_vector(self, word: str) -> np.ndarray:
        """Sözün vektorunu qaytarır. Cache istifadə edir."""
        word = word.strip().lower()
        if word in self._word_vectors_cache:
            return self._word_vectors_cache[word]

        if self.model:
            vec = self.model.get_word_vector(word)
        else:
            # Demo mode: deterministic pseudo-random vector
            np.random.seed(hash(word) % (2**32))
            vec = np.random.randn(300).astype(np.float32)

        self._word_vectors_cache[word] = vec
        return vec

    def cosine_similarity(self, word1: str, word2: str) -> float:
        """İki söz arasındakı cosine similarity (0-1 arası)."""
        vec1 = self.get_word_vector(word1)
        vec2 = self.get_word_vector(word2)

        dot = np.dot(vec1, vec2)
        norms = norm(vec1) * norm(vec2)

        if norms == 0:
            return 0.0

        sim = float(dot / norms)
        # -1..1 → 0..1 normalize
        return max(0.0, min(1.0, (sim + 1) / 2))

    def get_rank(self, guess: str, target: str, total_words: int = 10000) -> int:
        """
        Təxmin edilən sözün hədəf sözə olan sıra nömrəsini qaytarır.
        Rank 1 = ən yaxın, rank 10000 = ən uzaq.
        """
        similarity = self.cosine_similarity(guess, target)

        # Yüksək similarity = aşağı rank (yaxın)
        # Similarity 1.0 → rank 1, similarity 0.0 → rank 10000
        rank = max(1, int((1.0 - similarity) * total_words))

        # Mükəmməl uyğunluq
        if guess.strip().lower() == target.strip().lower():
            rank = 1

        return rank

    def get_similarity_and_rank(self, guess: str, target: str) -> tuple[float, int]:
        """Həm similarity, həm rank qaytarır."""
        similarity = self.cosine_similarity(guess, target)
        rank = self.get_rank(guess, target)
        return similarity, rank

    def get_color_category(self, rank: int) -> str:
        """Rank-a görə rəng kateqoriyası."""
        if rank <= 100:
            return "hot"      # Çox yaxın — yaşıl
        elif rank <= 500:
            return "warm"     # Yaxın — sarı
        elif rank <= 1000:
            return "cool"     # Orta — narıncı
        else:
            return "cold"     # Uzaq — qırmızı
