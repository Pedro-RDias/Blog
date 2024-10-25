let ingredientCount = 1;
let stepCount = 1;

function addIngredient() {
    const ingredientsDiv = document.getElementById('ingredients');
    const newIngredient = document.createElement('div');
    newIngredient.innerHTML = `
        <input type="text" class="ingredient-name form-control mt-2" placeholder="Ingredient name" required>
        <input type="text" class="ingredient-quantity form-control mt-2" placeholder="Quantity" required>
        <div class="form-check">
            <input class="form-check-input ingredient-allergen" type="checkbox" id="isAllergen${ingredientCount}">
            <label class="form-check-label" for="isAllergen${ingredientCount}">Is Allergen</label>
        </div>
        <button type="button" class="btn btn-danger btn-sm mt-1" onclick="removeElement(this)">Remove</button>
    `;
    ingredientsDiv.appendChild(newIngredient);
    ingredientCount++;
}

function addStep() {
    const stepsDiv = document.getElementById('steps');
    const newStep = document.createElement('div');
    newStep.innerHTML = `
        <input type="number" class="step-number form-control mt-2" placeholder="Step number" required min="1" max="50">
        <textarea class="step-description form-control mt-2" placeholder="Step description" required></textarea>
        <button type="button" class="btn btn-danger btn-sm mt-1" onclick="removeElement(this)">Remove</button>
    `;
    stepsDiv.appendChild(newStep);
    stepCount++;
}

function removeElement(button) {
    button.parentElement.remove();
}

function collectFormData() {
    const ingredients = Array.from(document.querySelectorAll('#ingredients > div')).map(div => ({
        Name: div.querySelector('.ingredient-name').value,
        Quantity: div.querySelector('.ingredient-quantity').value,
        IsAllergen: div.querySelector('.ingredient-allergen').checked
    }));

    const steps = Array.from(document.querySelectorAll('#steps > div')).map(div => ({
        StepNumber: div.querySelector('.step-number').value,
        Description: div.querySelector('.step-description').value
    }));

    document.getElementById('ingredientsJson').value = JSON.stringify(ingredients);
    document.getElementById('stepsJson').value = JSON.stringify(steps);
}

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('addIngredientBtn').addEventListener('click', addIngredient);
    document.getElementById('addStepBtn').addEventListener('click', addStep);
    document.getElementById('recipeForm').addEventListener('submit', function (e) {
        e.preventDefault();
        collectFormData();
        this.submit();
    });
});