const carousels = document.querySelectorAll('.popular-series-container');

carousels.forEach((carousel) => {
    const slides = carousel.querySelectorAll('.series-card');
    const prevButton = carousel.querySelector('.serial-carousel-button.prev');
    const nextButton = carousel.querySelector('.serial-carousel-button.next');

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

const movieCarousels = document.querySelectorAll('.popular-movies-container');

movieCarousels.forEach((carousel) => {
    const slides = carousel.querySelectorAll('.movie-card');
    const prevButton = carousel.querySelector('.movie-carousel-button.prev');
    const nextButton = carousel.querySelector('.movie-carousel-button.next');

    let currentIndex = 0;

    function updateDisplay() {
        slides.forEach((slide, index) => {
            if (index >= currentIndex && index < currentIndex + 5) {
                slide.style.display = 'block';
            } else {
                slide.style.display = 'none';
            }
        });

        if (currentIndex === 0) {
            prevButton.style.display = 'none';
        } else {
            prevButton.style.display = 'block';
        }

        if (currentIndex + 5 >= slides.length) {
            nextButton.style.display = 'none';
        } else {
            nextButton.style.display = 'block';
        }
    }

    prevButton.addEventListener('click', () => {
        currentIndex = Math.max(0, currentIndex - 5);
        updateDisplay();
    });

    nextButton.addEventListener('click', () => {
        currentIndex = Math.min(slides.length - 5, currentIndex + 5);
        updateDisplay();
    });

    updateDisplay();
});

const dramaCarousels = document.querySelectorAll('.drama-container');

dramaCarousels.forEach((carousel) => {
    const slides = carousel.querySelectorAll('.drama-card');
    const prevButton = carousel.querySelector('.drama-carousel-button.prev');
    const nextButton = carousel.querySelector('.drama-carousel-button.next');

    let currentIndex = 0;

    function updateDisplay() {
        slides.forEach((slide, index) => {
            if (index >= currentIndex && index < currentIndex + 5) {
                slide.style.display = 'block';
            } else {
                slide.style.display = 'none';
            }
        });

        if (currentIndex === 0) {
            prevButton.style.display = 'none';
        } else {
            prevButton.style.display = 'block';
        }

        if (currentIndex + 5 >= slides.length) {
            nextButton.style.display = 'none';
        } else {
            nextButton.style.display = 'block';
        }
    }

    prevButton.addEventListener('click', () => {
        currentIndex = Math.max(0, currentIndex - 5);
        updateDisplay();
    });

    nextButton.addEventListener('click', () => {
        currentIndex = Math.min(slides.length - 5, currentIndex + 5);
        updateDisplay();
    });

    updateDisplay();
});


const actionMovieCarousels = document.querySelectorAll('.action-movie-container');

actionMovieCarousels.forEach((carousel) => {
    const slides = carousel.querySelectorAll('.action-movie-card');
    const prevButton = carousel.querySelector('.action-movie-carousel-button.prev');
    const nextButton = carousel.querySelector('.action-movie-carousel-button.next');

    let currentIndex = 0;

    function updateDisplay() {
        slides.forEach((slide, index) => {
            if (index >= currentIndex && index < currentIndex + 5) {
                slide.style.display = 'block';
            } else {
                slide.style.display = 'none';
            }
        });

        if (currentIndex === 0) {
            prevButton.style.display = 'none';
        } else {
            prevButton.style.display = 'block';
        }

        if (currentIndex + 5 >= slides.length) {
            nextButton.style.display = 'none';
        } else {
            nextButton.style.display = 'block';
        }
    }

    prevButton.addEventListener('click', () => {
        currentIndex = Math.max(0, currentIndex - 5);
        updateDisplay();
    });

    nextButton.addEventListener('click', () => {
        currentIndex = Math.min(slides.length - 5, currentIndex + 5);
        updateDisplay();
    });

    updateDisplay();
});

const thrillersCarousels = document.querySelectorAll('.thrillers-container');

thrillersCarousels.forEach((carousel) => {
    const slides = carousel.querySelectorAll('.thrillers-card');
    const prevButton = carousel.querySelector('.thrillers-carousel-button.prev');
    const nextButton = carousel.querySelector('.thrillers-carousel-button.next');

    let currentIndex = 0;

    function updateDisplay() {
        slides.forEach((slide, index) => {
            if (index >= currentIndex && index < currentIndex + 5) {
                slide.style.display = 'block';
            } else {
                slide.style.display = 'none';
            }
        });

        if (currentIndex === 0) {
            prevButton.style.display = 'none';
        } else {
            prevButton.style.display = 'block';
        }

        if (currentIndex + 5 >= slides.length) {
            nextButton.style.display = 'none';
        } else {
            nextButton.style.display = 'block';
        }
    }

    prevButton.addEventListener('click', () => {
        currentIndex = Math.max(0, currentIndex - 5);
        updateDisplay();
    });

    nextButton.addEventListener('click', () => {
        currentIndex = Math.min(slides.length - 5, currentIndex + 5);
        updateDisplay();
    });

    updateDisplay();
});


