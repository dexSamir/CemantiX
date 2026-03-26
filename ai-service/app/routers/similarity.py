from fastapi import APIRouter, Request, Query
from app.models.similarity import (
    SimilarityRequest, SimilarityResponse,
    ThinkingScoreRequest, ThinkingScoreResponse,
    RandomWordResponse, RandomWordsResponse
)
from app.services.thinking_score import ThinkingScoreCalculator
from app.services.word_bank import WordBank

router = APIRouter()
word_bank = WordBank()


@router.post("/similarity", response_model=SimilarityResponse)
async def get_similarity(request: Request, body: SimilarityRequest):
    """Təxmin edilən sözün hədəf sözə semantik yaxınlığını hesablayır."""
    ft = request.app.state.fasttext
    similarity, rank = ft.get_similarity_and_rank(body.guess, body.target)
    color = ft.get_color_category(rank)

    return SimilarityResponse(
        word=body.guess,
        similarity=round(similarity, 4),
        rank=rank,
        color_category=color
    )


@router.post("/thinking-score", response_model=ThinkingScoreResponse)
async def get_thinking_score(body: ThinkingScoreRequest):
    """Oyunçunun düşünmə balını hesablayır."""
    guesses = [
        {
            "word": g.word,
            "similarity": g.similarity,
            "timestamp": g.timestamp
        }
        for g in body.guesses
    ]

    result = ThinkingScoreCalculator.calculate(guesses, body.duration_seconds)

    return ThinkingScoreResponse(
        speed_factor=result["speed_factor"],
        logic_factor=result["logic_factor"],
        consistency_factor=result["consistency_factor"],
        overall_score=result["overall_score"],
        grade=result["grade"]
    )


@router.get("/random-word", response_model=RandomWordResponse)
async def get_random_word(difficulty: str = Query("easy", enum=["easy", "medium", "hard"])):
    """Çətinlik dərəcəsinə görə random söz qaytarır."""
    word = word_bank.get_random_word(difficulty)
    return RandomWordResponse(word=word)


@router.get("/random-words", response_model=RandomWordsResponse)
async def get_random_words(
    difficulty: str = Query("easy", enum=["easy", "medium", "hard"]),
    count: int = Query(3, ge=1, le=5)
):
    """Multi-Target Contexto üçün əlaqəli sözlər qaytarır."""
    words = word_bank.get_random_words(difficulty, count)
    return RandomWordsResponse(words=words)
