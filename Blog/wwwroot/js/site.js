// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    const stars = document.querySelectorAll('.star');
    const recipeId = window.location.pathname.split('/').pop();
    const antiForgeryToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    
    // Load current rating
    fetch(`/api/Ratings/${recipeId}`)
        .then(response => response.json())
        .then(rating => {
            highlightStars(Math.round(rating));
        });

    stars.forEach(star => {
        star.addEventListener('click', function() {
            const ratingValue = parseInt(this.dataset.rating);
            
            fetch('/api/Ratings', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': antiForgeryToken
                },
                body: JSON.stringify({
                    recipeId: parseInt(recipeId),
                    ratingValue: ratingValue
                })
            })
            .then(async response => {
                if (response.ok) {
                    highlightStars(ratingValue);
                    document.getElementById('ratingMessage').textContent = 'Thank you for rating!';
                } else {
                    const errorData = await response.json();
                    throw new Error(errorData.title || 'Error saving rating');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                document.getElementById('ratingMessage').textContent = 'Error saving rating. Please try again.';
            });
        });
    });

    function highlightStars(rating) {
        stars.forEach(star => {
            if (star.dataset.rating <= rating) {
                star.textContent = '★';
            } else {
                star.textContent = '☆';
            }
        });
    }
});