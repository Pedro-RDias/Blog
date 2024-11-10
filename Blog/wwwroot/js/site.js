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

document.addEventListener("DOMContentLoaded", function() {
    // Seleciona todos os links de anos com a classe 'year-toggle'
    var yearToggles = document.querySelectorAll('.year-toggle');

    yearToggles.forEach(function(toggle) {
        toggle.addEventListener('click', function(event) {
            event.preventDefault(); // Previne o comportamento padrão do link

            // Seleciona a lista de meses e a seta associada ao ano específico
            var year = this.getAttribute('data-year');
            var monthList = document.getElementById('months-' + year);
            var arrow = this.querySelector('.arrow');

            // Alterna a visibilidade da lista de meses e a orientação da seta
            if (monthList.style.display === 'none' || monthList.style.display === '') {
                monthList.style.display = 'block';
                arrow.classList.remove('bi-chevron-right');
                arrow.classList.add('bi-chevron-down'); // Muda a seta para baixo
            } else {
                monthList.style.display = 'none';
                arrow.classList.remove('bi-chevron-down');
                arrow.classList.add('bi-chevron-right'); // Muda a seta para a direita
            }
        });
    });

    // Previne o fechamento do dropdown ao clicar em qualquer item dentro do dropdown
    document.querySelector('.dropdown-menu').addEventListener('click', function(event) {
        event.stopPropagation();
    });
});

document.addEventListener("DOMContentLoaded", function() {
    var searchInput = document.getElementById('searchInput');
    var searchIcon = document.getElementById('searchIcon');
    var selectedMonth = document.getElementById('selectedMonth');
    var selectedYear = document.getElementById('selectedYear');
    var selectedFilter = document.getElementById('selectedFilter');
    var filterText = document.getElementById('filterText');
    var clearFilter = document.getElementById('clearFilter');
    var filterDropdownMenu = document.querySelector('.dropdown-menu');
    var searchForm = document.querySelector('.search-bar form');

    function updateIcon() {
        if (searchInput.value.length > 0 || selectedMonth.value || selectedYear.value) {
            searchIcon.innerHTML = '<i class="bi bi-x-lg text-muted"></i>';
        } else {
            searchIcon.innerHTML = '<i class="bi bi-search text-muted"></i>';
        }
    }

    // Exibe o filtro de data selecionado na barra de pesquisa
    function showSelectedFilter() {
        if (selectedMonth.value && selectedYear.value) {
            const monthName = new Date(selectedYear.value, selectedMonth.value - 1).toLocaleString('default', { month: 'long' });
            filterText.textContent = `${monthName} ${selectedYear.value}`;
            selectedFilter.style.display = 'flex';
        } else {
            selectedFilter.style.display = 'none';
        }
    }

    // Atualiza o ícone quando o usuário digita
    searchInput.addEventListener('input', updateIcon);

    // Limpa o campo de pesquisa e o filtro de data ao clicar no "X"
    searchIcon.addEventListener('click', function() {
        if (searchInput.value.length > 0 || selectedMonth.value || selectedYear.value) {
            searchInput.value = '';
            selectedMonth.value = '';
            selectedYear.value = '';
            updateIcon();
            showSelectedFilter();
            // Removemos `searchForm.submit();` para evitar o reload
        }
    });

    // Limpa apenas o filtro de data ao clicar no "X" ao lado do filtro
    clearFilter.addEventListener('click', function() {
        selectedMonth.value = '';
        selectedYear.value = '';
        showSelectedFilter();
        // Removemos `searchForm.submit();` para evitar o reload
    });

    // Define o filtro de mês e ano ao clicar e exibe na barra de pesquisa
    filterDropdownMenu.addEventListener('click', function(event) {
        if (event.target.classList.contains('filter-month')) {
            selectedMonth.value = event.target.getAttribute('data-month');
            selectedYear.value = event.target.getAttribute('data-year');
            updateIcon();
            showSelectedFilter();
            // Aqui você pode manter `searchForm.submit()` caso precise aplicar o filtro imediatamente
        }
    });

    // Inicializa o ícone e o filtro exibido
    updateIcon();
    showSelectedFilter();
});

document.addEventListener("DOMContentLoaded", function() {
    const filterForm = document.querySelector('.search-bar form');
    const recipesSection = document.getElementById('recipes-section');
    const searchInput = document.getElementById('searchInput');
    const filterDropdownMenu = document.querySelector('.dropdown-menu');

    // Função para realizar o scroll para a seção de receitas
    function scrollToRecipesSection() {
        recipesSection.scrollIntoView({ behavior: 'smooth' });
    }

    // Adiciona evento de submissão do formulário para ativar o scroll
    filterForm.addEventListener('submit', function(event) {
        sessionStorage.setItem('scrollToRecipes', 'true'); // Armazena a intenção de rolar
    });

    // Detecta Enter no campo de pesquisa
    searchInput.addEventListener('keydown', function(event) {
        if (event.key === "Enter") {
            event.preventDefault();
            sessionStorage.setItem('scrollToRecipes', 'true');
            filterForm.submit();
        }
    });

    // Realiza scroll ao carregar a página, se o flag estiver definido
    if (sessionStorage.getItem('scrollToRecipes') === 'true') {
        sessionStorage.removeItem('scrollToRecipes');
        scrollToRecipesSection();
    }

    // Evento para aplicar o scroll automaticamente ao selecionar um filtro de mês/ano
    filterDropdownMenu.addEventListener('click', function(event) {
        if (event.target.classList.contains('filter-month')) {
            event.preventDefault();
            const selectedMonth = document.getElementById('selectedMonth');
            const selectedYear = document.getElementById('selectedYear');

            // Define o valor do mês e ano no formulário
            selectedMonth.value = event.target.getAttribute('data-month');
            selectedYear.value = event.target.getAttribute('data-year');

            // Define o flag de scroll e submete o formulário
            sessionStorage.setItem('scrollToRecipes', 'true');
            filterForm.submit();
        }
    });
});