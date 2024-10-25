let ingredientCount = 1;
let stepCount = 1;

function addIngredient(ingredient = null) {
    const ingredientsDiv = document.getElementById('ingredients');
    const newIngredient = document.createElement('div');
    newIngredient.innerHTML = `
        <input type="hidden" class="ingredient-id" value="${ingredient?.IngredientId || '0'}">
        <input type="text" 
            class="ingredient-name form-control mt-2" 
            placeholder="Ingredient name" 
            value="${ingredient?.Name || ''}"
            required>
        <input type="text" 
            class="ingredient-quantity form-control mt-2" 
            placeholder="Quantity" 
            value="${ingredient?.Quantity || ''}"
            required>
        <div class="form-check">
            <input class="form-check-input ingredient-allergen" 
                type="checkbox" 
                id="isAllergen${ingredientCount}"
                ${ingredient?.IsAllergen ? 'checked' : ''}>
            <label class="form-check-label" for="isAllergen${ingredientCount}">Is Allergen</label>
        </div>
        <button type="button" class="btn btn-danger btn-sm mt-1" onclick="removeElement(this)">Remove</button>
    `;
    ingredientsDiv.appendChild(newIngredient);
    ingredientCount++;
}

function addStep(step = null) {
    const stepsDiv = document.getElementById('steps');
    const newStep = document.createElement('div');
    newStep.innerHTML = `
        <input type="hidden" class="step-id" value="${step?.SetpId || '0'}">
        <input type="number" 
            class="step-number form-control mt-2" 
            placeholder="Step number" 
            value="${step?.StepNumber || ''}"
            required min="1" max="50">
        <textarea class="step-description form-control mt-2" 
            placeholder="Step description" 
            required>${step?.Description || ''}</textarea>
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
        IngredientId: parseInt(div.querySelector('.ingredient-id').value),
        Name: div.querySelector('.ingredient-name').value,
        Quantity: div.querySelector('.ingredient-quantity').value,
        IsAllergen: div.querySelector('.ingredient-allergen').checked
    }));

    const steps = Array.from(document.querySelectorAll('#steps > div')).map(div => ({
        SetpId: parseInt(div.querySelector('.step-id').value),
        StepNumber: parseInt(div.querySelector('.step-number').value),
        Description: div.querySelector('.step-description').value
    }));

    // Sort steps by step number
    steps.sort((a, b) => a.StepNumber - b.StepNumber);

    document.getElementById('ingredientsJson').value = JSON.stringify(ingredients);
    document.getElementById('stepsJson').value = JSON.stringify(steps);
}

function populateFormData() {
    const ingredientsJson = document.getElementById('ingredientsJson').value;
    const stepsJson = document.getElementById('stepsJson').value;

    try {
        if (ingredientsJson) {
            const ingredients = JSON.parse(ingredientsJson);
            console.log('Loading ingredients:', ingredients);
            ingredients.forEach(ingredient => addIngredient(ingredient));
        }

        if (stepsJson) {
            const steps = JSON.parse(stepsJson);
            console.log('Loading steps:', steps);
            // Sort steps by step number before populating
            steps.sort((a, b) => a.StepNumber - b.StepNumber);
            steps.forEach(step => addStep(step));
        }
    } catch (error) {
        console.error('Error populating form data:', error);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('addIngredientBtn').addEventListener('click', () => addIngredient());
    document.getElementById('addStepBtn').addEventListener('click', () => addStep());
    document.getElementById('recipeForm').addEventListener('submit', function (e) {
        e.preventDefault();
        collectFormData();
        this.submit();
    });

    // If there are no ingredients/steps added yet, populate from hidden fields
    if (document.querySelectorAll('#ingredients > div').length === 0 &&
        document.querySelectorAll('#steps > div').length === 0) {
        populateFormData();
    }
});