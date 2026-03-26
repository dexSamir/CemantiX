from fastapi import APIRouter

router = APIRouter()


@router.get("/health")
async def health_check():
    """Sağlamlıq yoxlaması."""
    return {
        "status": "healthy",
        "service": "SemantiX AI Service",
        "version": "1.0.0"
    }
