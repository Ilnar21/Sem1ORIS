const carousels = document.querySelectorAll('.popular-series-container, .popular-movies-container, .drama-container, .action-movie-container, .thrillers-container');

carousels.forEach((carousel) => {
    const type = carousel.classList[0].split('-')[0]; // Получаем тип: 'popular', 'drama', 'action', 'thrillers'
    const slides = carousel.querySelectorAll(`.${type}-card`);
    const prevButton = carousel.querySelector(`.${type}-carousel-button.prev`);
    const nextButton = carousel.querySelector(`.${type}-carousel-button.next`);

    let currentIndex = 0;

    function updateDisplay() {
        slides.forEach((slide, index) => {
            if (index >= currentIndex && index < currentIndex + 5) {
                slide.style.display = 'block';
            } else {
                slide.style.display = 'none';
            }
        });

        prevButton.style.display = currentIndex === 0 ? 'none' : 'block';
        nextButton.style.display = currentIndex + 5 >= slides.length ? 'none' : 'block';
    }

    prevButton.addEventListener('click', () => {
        currentIndex = Math.max(0, currentIndex - 5);
        updateDisplay();
    });

    nextButton.addEventListener('click', () => {
        currentIndex = Math.min(slides.length - 5, currentIndex + 5);
        updateDisplay();
    });

    // Инициализация отображения
    updateDisplay();
});
