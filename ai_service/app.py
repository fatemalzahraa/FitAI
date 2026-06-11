from fastapi import FastAPI
from pydantic import BaseModel
import joblib

app = FastAPI()

model = joblib.load("model.pkl")

class ReviewRequest(BaseModel):
    text: str

@app.get("/")
def root():
    return {"message": "FitAI NLP Service Running"}

@app.post("/predict")
def predict(request: ReviewRequest):
    result = model.predict([request.text])[0]

    return {
        "text": request.text,
        "prediction": result
    }

@app.post("/review-analysis")
def review_analysis(reviews: list[str]):

    positive = 0
    negative = 0

    issues = []

    for review in reviews:

        result = model.predict([review])[0]

        if result == "pozitif":
            positive += 1
        else:
            negative += 1

            text = review.lower()

            if "dar" in text:
                issues.append("Dar kalıp")

            if "ince" in text:
                issues.append("Kumaş ince")

            if "kısa" in text:
                issues.append("Boy kısa")

    total = positive + negative

    score = round((positive / total) * 100) if total > 0 else 0

    return {
        "fitScore": score,
        "positiveReviews": positive,
        "negativeReviews": negative,
        "issues": list(set(issues))
    }