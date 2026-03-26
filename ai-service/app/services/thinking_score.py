from datetime import datetime
import math
import logging

logger = logging.getLogger(__name__)


class ThinkingScoreCalculator:
    """
    Oyunçunun "Düşünmə Balı"nı hesablayır.
    
    Üç amil nəzərə alınır:
    1. Speed Factor — Sürətli tapma qabiliyyəti
    2. Logic Factor — Təxminlər arasındakı məntiqi keçidlərin keyfiyyəti
    3. Consistency Factor — Ardıcıl yaxınlaşma qabiliyyəti
    """

    @staticmethod
    def calculate(
        guesses: list[dict],  # [{word, similarity, timestamp}, ...]
        duration_seconds: float
    ) -> dict:
        if not guesses:
            return {
                "speed_factor": 0.0,
                "logic_factor": 0.0,
                "consistency_factor": 0.0,
                "overall_score": 0.0,
                "grade": "🎲 Şanslı"
            }

        # === 1. Speed Factor ===
        # Az cəhd + qısa vaxt = yüksək sürət balı
        attempt_count = len(guesses)
        time_factor = max(0, 100 - (duration_seconds / 3))  # 300s-dən sonra 0
        attempt_factor = max(0, 100 - (attempt_count * 3))   # 33 cəhddən sonra 0
        speed_factor = (time_factor * 0.5 + attempt_factor * 0.5)

        # === 2. Logic Factor ===
        # Hər təxminin əvvəlkinə nəzərən daha məntiqi olub-olmadığı
        # Similarity artırsa = yaxşı, azalırsa = pis
        logic_scores = []
        sorted_guesses = sorted(guesses, key=lambda g: g.get("timestamp", ""))
        similarities = [g["similarity"] for g in sorted_guesses]

        for i in range(1, len(similarities)):
            diff = similarities[i] - similarities[i - 1]
            if diff > 0:
                # Yaxınlaşma — yaxşı keçid
                logic_scores.append(min(1.0, diff * 5))
            elif diff < 0:
                # Uzaqlaşma — pis keçid (amma çox cəzalandırma)
                logic_scores.append(max(-0.5, diff * 2))
            else:
                logic_scores.append(0)

        if logic_scores:
            avg_logic = sum(logic_scores) / len(logic_scores)
            logic_factor = max(0, min(100, 50 + avg_logic * 50))
        else:
            logic_factor = 50.0

        # === 3. Consistency Factor ===
        # Nə qədər az "geri-irəli" gedirsə, o qədər ardıcıldır
        direction_changes = 0
        for i in range(2, len(similarities)):
            prev_diff = similarities[i - 1] - similarities[i - 2]
            curr_diff = similarities[i] - similarities[i - 1]
            if (prev_diff > 0 and curr_diff < 0) or (prev_diff < 0 and curr_diff > 0):
                direction_changes += 1

        if len(similarities) >= 3:
            change_ratio = direction_changes / (len(similarities) - 2)
            consistency_factor = max(0, 100 * (1 - change_ratio))
        else:
            consistency_factor = 70.0  # Default

        # === Overall ===
        overall = speed_factor * 0.3 + logic_factor * 0.4 + consistency_factor * 0.3

        # Grade
        grade = ThinkingScoreCalculator._get_grade(overall)

        return {
            "speed_factor": round(speed_factor, 1),
            "logic_factor": round(logic_factor, 1),
            "consistency_factor": round(consistency_factor, 1),
            "overall_score": round(overall, 1),
            "grade": grade
        }

    @staticmethod
    def _get_grade(score: float) -> str:
        if score >= 80:
            return "🧠 Dahi"
        elif score >= 60:
            return "⚡ Sürətli Düşünən"
        elif score >= 40:
            return "🎯 Məntiqli"
        elif score >= 20:
            return "🐢 Tədricən"
        else:
            return "🎲 Şanslı"
