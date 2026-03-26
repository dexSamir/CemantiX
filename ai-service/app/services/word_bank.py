import random
import os
import logging

logger = logging.getLogger(__name__)

# Çətinlik dərəcəsinə görə Azərbaycan dilində söz bankı
WORD_BANK = {
    "easy": [
        "ev", "su", "od", "yol", "gün", "göz", "əl", "baş", "dağ", "dəniz",
        "çörək", "alma", "ağac", "günəş", "ay", "ulduz", "yağış", "qar",
        "külək", "torpaq", "çiçək", "quş", "balıq", "pişik", "it",
        "uşaq", "ana", "ata", "qardaş", "bacı", "kitab", "məktəb",
        "müəllim", "şagird", "sinif", "dərs", "qələm", "kağız",
        "stol", "stul", "qapı", "pəncərə", "divar", "dam", "həyət",
        "bağ", "meşə", "çay", "göl", "dərə", "bulud", "səma",
        "rəng", "mavi", "qırmızı", "yaşıl", "sarı", "ağ", "qara",
        "böyük", "kiçik", "uzun", "qısa", "yeni", "köhnə",
        "maşın", "avtobus", "qatar", "təyyarə", "gəmi",
    ],
    "medium": [
        "azadlıq", "demokratiya", "mədəniyyət", "tarix", "coğrafiya",
        "elm", "texnologiya", "kompüter", "internet", "telefon",
        "universitet", "professor", "tələbə", "imtahan", "diploma",
        "xəstəxana", "həkim", "əczaçı", "dərman", "müalicə",
        "hökumət", "prezident", "nazir", "qanun", "konstitusiya",
        "iqtisadiyyat", "bank", "valyuta", "bazaar", "ticarət",
        "futbol", "voleybol", "basketbol", "üzgüçülük", "güləş",
        "musiqi", "rəssamlıq", "heykəltəraşlıq", "teatro", "kino",
        "restoran", "mətbəx", "resept", "dadlı", "ləziz",
        "səyahət", "turizm", "otel", "bilet", "pasport",
        "ailə", "toy", "bayram", "hədiyyə", "sevinc",
        "məhəbbət", "dostluq", "sülh", "ədalət", "həqiqət",
        "müharibə", "barış", "diplomat", "səfir", "danışıq",
    ],
    "hard": [
        "epistemologiya", "ontologiya", "hermenevtika", "fenomenologiya",
        "psixoanaliz", "ekzistensializm", "postmodernizm", "strukturalizm",
        "kvant", "termodinamika", "elektromaqnetizm", "gravitasiya",
        "fotosintez", "mitoxondri", "xromosom", "genetika",
        "alqoritm", "kriptografiya", "blockchain", "neyroşəbəkə",
        "antropologiya", "arxeologiya", "paleontologiya", "etimologiya",
        "geopolitika", "globalizasiya", "urbanizasiya", "industrializasiya",
        "ekosistem", "biodiversitet", "iqlim", "atmosfer",
        "həndəsə", "triqonometriya", "differensial", "inteqral",
        "infrastruktur", "arxitektura", "renovasiya", "restavrasiya",
        "diplomatiya", "müstəqillik", "suverenlik", "federasiya",
    ]
}


class WordBank:
    """Azərbaycan dilində söz bankı idarəetmə servisi."""

    def __init__(self, words_file: str | None = None):
        self.words = dict(WORD_BANK)  # Copy
        if words_file and os.path.exists(words_file):
            self._load_from_file(words_file)

    def _load_from_file(self, filepath: str):
        """Xarici söz faylından əlavə sözlər yüklə."""
        try:
            with open(filepath, "r", encoding="utf-8") as f:
                for line in f:
                    parts = line.strip().split("|")
                    if len(parts) == 2:
                        difficulty, word = parts[0].strip(), parts[1].strip()
                        if difficulty in self.words:
                            self.words[difficulty].append(word)
            logger.info(f"Xarici söz bankı yükləndi: {filepath}")
        except Exception as e:
            logger.error(f"Söz bankı yüklənərkən xəta: {e}")

    def get_random_word(self, difficulty: str = "easy") -> str:
        """Çətinlik dərəcəsinə görə random söz qaytarır."""
        diff = difficulty.lower()
        if diff not in self.words:
            diff = "easy"
        return random.choice(self.words[diff])

    def get_random_words(self, difficulty: str = "easy", count: int = 3) -> list[str]:
        """
        Multi-Target Contexto üçün əlaqəli sözlər qaytarır.
        Eyni kateqoriyadan count qədər fərqli söz seçilir.
        """
        diff = difficulty.lower()
        if diff not in self.words:
            diff = "easy"

        pool = list(self.words[diff])
        count = min(count, len(pool))
        return random.sample(pool, count)

    def get_all_words(self, difficulty: str | None = None) -> list[str]:
        """Bütün sözləri qaytarır."""
        if difficulty:
            return self.words.get(difficulty.lower(), [])
        all_words = []
        for word_list in self.words.values():
            all_words.extend(word_list)
        return all_words
