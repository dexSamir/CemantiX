from pydantic import BaseModel
from typing import Optional


class SimilarityRequest(BaseModel):
    guess: str
    target: str


class SimilarityResponse(BaseModel):
    word: str
    similarity: float
    rank: int
    color_category: str


class ThinkingScoreGuess(BaseModel):
    word: str
    similarity: float
    timestamp: str


class ThinkingScoreRequest(BaseModel):
    guesses: list[ThinkingScoreGuess]
    duration_seconds: float


class ThinkingScoreResponse(BaseModel):
    speed_factor: float
    logic_factor: float
    consistency_factor: float
    overall_score: float
    grade: str


class RandomWordResponse(BaseModel):
    word: str


class RandomWordsResponse(BaseModel):
    words: list[str]
