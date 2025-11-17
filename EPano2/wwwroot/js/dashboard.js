// ISUBU Digital Information Board JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Initialize all dashboard components
    initializeDateTime();
    initializeVideoPlayer();
    initializeAnnouncementsCarousel();
    initializeScrollingText();
    initializeWeatherAPI();
    
    // Add fade-in animation to main elements
    addFadeInAnimations();
});

// Real-time Date and Time Updates
function initializeDateTime() {
    function updateDateTime() {
        const now = new Date();
        const timeElement = document.getElementById('datetime-time');
        const dateElement = document.getElementById('datetime-date');
        
        if (timeElement) {
            const timeString = now.toLocaleTimeString('tr-TR', {
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit'
            });
            timeElement.textContent = timeString;
        }
        
        if (dateElement) {
            const dateString = now.toLocaleDateString('tr-TR', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
            dateElement.textContent = dateString;
        }
    }
    
    // Update immediately and then every second
    updateDateTime();
    setInterval(updateDateTime, 1000);
}

// Video Player with Auto-rotation
function initializeVideoPlayer() {
    const videos = document.querySelectorAll('.video-player video');
    let currentVideoIndex = 0;
    
    if (videos.length === 0) return;
    
    // Hide all videos except the first one
    videos.forEach((video, index) => {
        if (index === 0) {
            video.style.display = 'block';
            video.classList.add('active');
        } else {
            video.style.display = 'none';
        }
    });
    
    function switchToNextVideo() {
        // Hide current video
        videos[currentVideoIndex].style.display = 'none';
        videos[currentVideoIndex].classList.remove('active');
        
        // Move to next video
        currentVideoIndex = (currentVideoIndex + 1) % videos.length;
        
        // Show next video
        videos[currentVideoIndex].style.display = 'block';
        videos[currentVideoIndex].classList.add('active');
        
        // Play the video
        videos[currentVideoIndex].play().catch(e => {
            console.log('Video autoplay prevented:', e);
        });
    }
    
    // Switch videos every 30 seconds
    setInterval(switchToNextVideo, 30000);
    
    // Handle video end event
    videos.forEach(video => {
        video.addEventListener('ended', function() {
            setTimeout(switchToNextVideo, 2000); // Wait 2 seconds before switching
        });
    });
}

// Announcements Carousel
function initializeAnnouncementsCarousel() {
    const slides = document.querySelectorAll('.announcement-slide');
    let currentSlideIndex = 0;
    
    if (slides.length === 0) return;
    
    // Show first slide
    slides[0].classList.add('active');
    
    function switchToNextSlide() {
        // Remove active class from current slide
        slides[currentSlideIndex].classList.remove('active');
        slides[currentSlideIndex].classList.add('prev');
        
        // Move to next slide
        currentSlideIndex = (currentSlideIndex + 1) % slides.length;
        
        // Show next slide
        setTimeout(() => {
            slides.forEach(slide => slide.classList.remove('prev'));
            slides[currentSlideIndex].classList.add('active');
        }, 500);
    }
    
    // Switch slides every 8 seconds
    setInterval(switchToNextSlide, 8000);
}

// Scrolling Text Animation
function initializeScrollingText() {
    const scrollingElements = document.querySelectorAll('.scrolling-text, .scrolling-text-credits');
    
    scrollingElements.forEach(element => {
        // Duplicate content for seamless scrolling
        const content = element.textContent;
        element.innerHTML = content + ' • ' + content + ' • ' + content;
        
        // Add hover pause functionality
        element.addEventListener('mouseenter', function() {
            this.style.animationPlayState = 'paused';
        });
        
        element.addEventListener('mouseleave', function() {
            this.style.animationPlayState = 'running';
        });
    });
}

// OpenWeatherMap API Integration
function initializeWeatherAPI() {
    fetch('/Home/Weather7')
        .then(res => res.json())
        .then(data => {
            if (!data || data.length === 0) return;
            renderWeatherData(data);
        })
        .catch(err => console.error('Weather fetch error:', err));
}

//async function fetchWeather(apiKey, lat, lon) {
//    try {
//        const response = await fetch(
//            `https://api.openweathermap.org/data/2.5/onecall?lat=${lat}&lon=${lon}&exclude=hourly,minutely,alerts&units=metric&lang=tr&appid=${apiKey}`
//        );
        
//        if (!response.ok) {
//            throw new Error(`HTTP error! status: ${response.status}`);
//        }
        
//        const data = await response.json();
//        renderWeather(data.daily.slice(0, 7)); // Use 7-day forecast
//    } catch (error) {
//        console.error('Error fetching weather data:', error);
//        // Fallback to mock data if API fails
//        renderMockWeather();
//    }
//}

function renderWeatherData(data) {
    const box = document.getElementById('weatherToday');
    if (!box) return;

    box.innerHTML = ''; // Eski veriyi temizle
    const { iconUrl, tempC } = data[0]; // Tek günlük veri alıyoruz

    box.innerHTML = `
        <img class="wt-icon" src="${iconUrl}" alt="hava">
        <div class="wt-temp">${tempC}°C</div>
    `;
}


//function renderMockWeather() {
//    const container = document.querySelector(".weather-container");
//    if (!container) return;
    
//    container.innerHTML = "";
    
//    const mockData = [
//        { icon: "10d", temp: 18 },
//        { icon: "04d", temp: 20 },
//        { icon: "02d", temp: 19 },
//        { icon: "01d", temp: 22 }, // Current day
//        { icon: "01d", temp: 24 },
//        { icon: "01d", temp: 26 },
//        { icon: "02d", temp: 23 }
//    ];
    
//    mockData.forEach((day, index) => {
//        const dayDiv = document.createElement("div");
//        dayDiv.className = "weather-day";
        
//        if (index === 3) {
//            dayDiv.classList.add("current");
//        }
        
//        dayDiv.innerHTML = `
//            <img src="https://openweathermap.org/img/wn/${day.icon}@2x.png" alt="weather" loading="lazy">
//            <p>${day.temp}°C</p>
//        `;
        
//        container.appendChild(dayDiv);
//    });
    
//    addWeatherAnimations();
//}

function addWeatherAnimations() {
    const weatherDays = document.querySelectorAll('.weather-day');
    
    weatherDays.forEach((day) => {
        day.addEventListener('mouseenter', function() {
            if (!this.classList.contains('current')) {
                this.style.transform = 'scale(1.1) translateY(-5px)';
                this.style.opacity = '1';
                this.style.zIndex = '20';
            }
        });
        
        day.addEventListener('mouseleave', function() {
            if (!this.classList.contains('current')) {
                this.style.transform = 'scale(0.8)';
                this.style.opacity = '0.6';
                this.style.zIndex = '1';
            }
        });
        
        day.addEventListener('click', function() {
            this.style.transform = this.style.transform.replace(/scale\([^)]*\)/, '') + ' scale(0.7)';
            setTimeout(() => {
                if (this.classList.contains('current')) {
                    this.style.transform = 'scale(1.3) translateY(-10px)';
                } else {
                    this.style.transform = 'scale(0.8)';
                }
            }, 150);
        });
    });
    
    // Add pulsing animation to current day
    const currentDay = document.querySelector('.weather-day.current');
    if (currentDay) {
        setInterval(() => {
            currentDay.style.transform = 'scale(1.4) translateY(-10px)';
            setTimeout(() => {
                currentDay.style.transform = 'scale(1.3) translateY(-10px)';
            }, 1000);
        }, 3000);
    }
}

// Add fade-in animations to elements
function addFadeInAnimations() {
    const elements = document.querySelectorAll('.dashboard-header, .video-section, .announcements-section, .weather-section, .datetime-section');
    
    elements.forEach((element, index) => {
        element.style.opacity = '0';
        element.style.transform = 'translateY(30px)';
        
        setTimeout(() => {
            element.style.transition = 'all 0.6s ease';
            element.style.opacity = '1';
            element.style.transform = 'translateY(0)';
        }, index * 200);
    });
}

// Weather Update Simulation (for demo purposes)
function updateWeather() {
    const weatherIcon = document.getElementById('weather-icon');
    const weatherTemp = document.getElementById('weather-temp');
    const weatherCondition = document.getElementById('weather-condition');
    
    if (!weatherIcon || !weatherTemp || !weatherCondition) return;
    
    // Simulate weather changes every 5 minutes
    const weatherData = [
        { icon: '☀️', temp: 25, condition: 'Güneşli' },
        { icon: '⛅', temp: 22, condition: 'Parçalı Bulutlu' },
        { icon: '☁️', temp: 18, condition: 'Bulutlu' },
        { icon: '🌧️', temp: 15, condition: 'Yağmurlu' },
        { icon: '❄️', temp: 5, condition: 'Karlı' }
    ];
    
    let currentWeatherIndex = 0;
    
    setInterval(() => {
        const weather = weatherData[currentWeatherIndex];
        weatherIcon.textContent = weather.icon;
        weatherTemp.textContent = weather.temp + '°C';
        weatherCondition.textContent = weather.condition;
        
        currentWeatherIndex = (currentWeatherIndex + 1) % weatherData.length;
    }, 300000); // 5 minutes
}

// Admin Panel Functions
function initializeAdminPanel() {
    // Add click handlers for admin buttons
    const addButtons = document.querySelectorAll('.btn-add');
    const editButtons = document.querySelectorAll('.btn-edit');
    const deleteButtons = document.querySelectorAll('.btn-delete');
    
    addButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            showModal('add');
        });
    });
    
    editButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const id = this.dataset.id;
            showModal('edit', id);
        });
    });
    
    deleteButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            e.preventDefault();
            const id = this.dataset.id;
            if (confirm('Bu öğeyi silmek istediğinizden emin misiniz?')) {
                deleteItem(id);
            }
        });
    });
}

// Modal Functions
function showModal(type, id = null) {
    // Create modal HTML
    const modalHtml = `
        <div class="modal-overlay" onclick="closeModal()">
            <div class="modal-content" onclick="event.stopPropagation()">
                <div class="modal-header">
                    <h3>${type === 'add' ? 'Yeni Ekle' : 'Düzenle'}</h3>
                    <button class="modal-close" onclick="closeModal()">&times;</button>
                </div>
                <div class="modal-body">
                    <form id="modal-form">
                        <div class="form-group">
                            <label>Başlık:</label>
                            <input type="text" name="title" required>
                        </div>
                        <div class="form-group">
                            <label>İçerik:</label>
                            <textarea name="content" rows="4"></textarea>
                        </div>
                        <div class="form-group">
                            <label>URL:</label>
                            <input type="url" name="url">
                        </div>
                        <div class="form-actions">
                            <button type="submit" class="btn-primary">Kaydet</button>
                            <button type="button" class="btn-secondary" onclick="closeModal()">İptal</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    `;
    
    document.body.insertAdjacentHTML('beforeend', modalHtml);
    
    // Add form submission handler
    document.getElementById('modal-form').addEventListener('submit', function(e) {
        e.preventDefault();
        saveItem(type, id);
    });
}

function closeModal() {
    const modal = document.querySelector('.modal-overlay');
    if (modal) {
        modal.remove();
    }
}

function saveItem(type, id) {
    // Mock save functionality
    console.log(`${type} item`, id);
    alert(`${type === 'add' ? 'Yeni öğe eklendi' : 'Öğe güncellendi'}!`);
    closeModal();
}

function deleteItem(id) {
    // Mock delete functionality
    console.log('Delete item', id);
    alert('Öğe silindi!');
}

// Utility Functions
function formatDate(date) {
    return new Date(date).toLocaleDateString('tr-TR');
}

function formatDateTime(date) {
    return new Date(date).toLocaleString('tr-TR');
}

// Error Handling
window.addEventListener('error', function(e) {
    console.error('Dashboard Error:', e.error);
});

// Performance Monitoring
function logPerformance() {
    if (window.performance) {
        const loadTime = window.performance.timing.loadEventEnd - window.performance.timing.navigationStart;
        console.log(`Dashboard loaded in ${loadTime}ms`);
    }
}

// Initialize performance logging
window.addEventListener('load', logPerformance);

// Export functions for global access
window.Dashboard = {
    switchToNextVideo: function() {
        const videos = document.querySelectorAll('.video-player video');
        if (videos.length > 0) {
            // Trigger video switch
            videos[0].dispatchEvent(new Event('ended'));
        }
    },
    switchToNextSlide: function() {
        const slides = document.querySelectorAll('.announcement-slide');
        if (slides.length > 0) {
            // Trigger slide switch
            slides[0].dispatchEvent(new Event('click'));
        }
    },
    showModal,
    closeModal,
    saveItem,
    deleteItem
};
