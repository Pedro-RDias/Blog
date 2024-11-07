let ingredientCount = 1;
let stepCount = 1;

function addIngredient(ingredient = null) {
    const ingredientsDiv = document.getElementById('ingredients');
    const newIngredient = document.createElement('div');
    newIngredient.innerHTML = `
        <input type="text" 
            class="ingredient-name form-control mt-2" 
            placeholder="Ingredient name" 
            value="${ingredient ? ingredient.Name : ''}"
            required>
        <input type="text" 
            class="ingredient-quantity form-control mt-2" 
            placeholder="Quantity" 
            value="${ingredient ? ingredient.Quantity : ''}"
            required>
        <div class="form-check">
            <input class="form-check-input ingredient-allergen" 
                type="checkbox" 
                id="isAllergen${ingredientCount}"
                ${ingredient && ingredient.IsAllergen ? 'checked' : ''}>
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
        <textarea class="step-description form-control mt-2" 
            placeholder="Step description" 
            required>${step ? step.Description : ''}</textarea>
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
        Description: div.querySelector('.step-description').value
    }));

    document.getElementById('ingredientsJson').value = JSON.stringify(ingredients);
    document.getElementById('stepsJson').value = JSON.stringify(steps);
}

function populateFormData() {
    const ingredientsJson = document.getElementById('ingredientsJson').value;
    const stepsJson = document.getElementById('stepsJson').value;

    if (ingredientsJson) {
        const ingredients = JSON.parse(ingredientsJson);
        ingredients.forEach(ingredient => addIngredient(ingredient));
    }

    if (stepsJson) {
        const steps = JSON.parse(stepsJson);
        steps.forEach(step => addStep(step));
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

    // Populate data from hidden fields if there are no ingredients/steps added yet
    if (document.querySelectorAll('#ingredients > div').length === 0 && document.querySelectorAll('#steps > div').length === 0) {
        populateFormData();
    }
});


function addPhotoUpload() {
    const container = document.getElementById('photoUploadContainer');
    const newRow = document.createElement('div');
    newRow.className = 'photo-upload-row mb-3';
    newRow.innerHTML = `
        <input type="file" name="photoFiles" class="form-control" accept="image/*" />
    `;
    container.appendChild(newRow);
}

