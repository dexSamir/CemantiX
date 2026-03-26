from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from contextlib import asynccontextmanager
import logging

from app.services.fasttext_service import FastTextService
from app.routers import similarity, health

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

# Global model instance
fasttext_svc: FastTextService | None = None

@asynccontextmanager
async def lifespan(app: FastAPI):
    """Tətbiq başladıqda model yüklənir, bağlandıqda resurs azad olunur."""
    global fasttext_svc
    logger.info("🚀 fastText modeli yüklənir...")
    fasttext_svc = FastTextService()
    fasttext_svc.load_model()
    logger.info("✅ Model yükləndi!")
    
    # State-ə əlavə et
    app.state.fasttext = fasttext_svc
    yield
    
    logger.info("🔴 Tətbiq bağlanır...")

app = FastAPI(
    title="SemantiX AI Service",
    description="Azərbaycan dilində semantik söz yaxınlığı hesablayan AI servis",
    version="1.0.0",
    lifespan=lifespan
)

# CORS
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Routers
app.include_router(similarity.router, tags=["Similarity"])
app.include_router(health.router, tags=["Health"])
