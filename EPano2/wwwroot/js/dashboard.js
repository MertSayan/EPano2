// ISUBU Digital Information Board JavaScript
document.addEventListener('DOMContentLoaded', function() {
    // Initialize all dashboard components
    initializeDateTime();
    initializeVideoPlayer();
    
    initializeAnnouncementsCarousel(); // Slider extracts data first
    
    // Initialize ticker AFTER carousel so it can use the same extracted data
    setTimeout(function() {
        initializeIndependentTicker(); // Independent ticker that loops between announcements and news
    }, 200);
    initializeScrollingText(); // Only handles credits scrolling
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

// Video Player with Auto-rotation (HTML5 video)
function initializeVideoPlayer() {
    const videoPlayer = document.getElementById('mainVideoPlayer');
    if (!videoPlayer) {
        return;
    }

    const videoFilePaths = (window.videoFilePaths || []).slice();
    const defaultVideoFilePath = window.defaultVideoFilePath;

    let currentIndex = 0;
    let isPlayingDefault = false;

    function setVideoSource(filePath) {
        if (!filePath) {
            console.warn('Video dosya yolu bulunamadı');
            return;
        }

        console.log('Switching video to:', filePath);
        videoPlayer.src = filePath;
        videoPlayer.load();
        
        // Video yüklendikten sonra oynatmayı başlat
        videoPlayer.addEventListener('loadeddata', function onLoaded() {
            videoPlayer.play().catch(err => {
                console.warn('Video oynatma hatası:', err);
            });
            videoPlayer.removeEventListener('loadeddata', onLoaded);
        }, { once: true });
    }

    // Video bittiğinde sıradaki videoya geç
    videoPlayer.addEventListener('ended', function() {
        if (videoFilePaths.length > 0) {
            // Aktif ekstra videolar varsa sıradakine geç (varsayılan video değil)
            isPlayingDefault = false;
            currentIndex = (currentIndex + 1) % videoFilePaths.length;
            setVideoSource(videoFilePaths[currentIndex]);
        } else if (defaultVideoFilePath) {
            // Aktif ekstra video yoksa varsayılan videoyu tekrar oynat (loop)
            isPlayingDefault = true;
            setVideoSource(defaultVideoFilePath);
        }
    });

    // İlk videoyu başlat
    // MANTIK: Aktif ekstra videolar varsa onlar sırayla döner, yoksa varsayılan video döner
    if (videoFilePaths.length > 0) {
        // Aktif ekstra videolar varsa ilkini oynat (varsayılan video değil)
        isPlayingDefault = false;
        currentIndex = 0;
        setVideoSource(videoFilePaths[0]);
    } else if (defaultVideoFilePath) {
        // Aktif ekstra video yoksa varsayılan videoyu oynat
        isPlayingDefault = true;
        setVideoSource(defaultVideoFilePath);
    } else {
        console.warn('Hiç video dosyası bulunamadı');
    }
}

// Announcements Carousel
function initializeAnnouncementsCarousel() {
    const carousel = document.querySelector('.announcements-carousel');
    if (!carousel) return;
    
    const allSlides = Array.from(document.querySelectorAll('.announcement-slide'));
    if (allSlides.length === 0) return;
    
        // Extract and store all announcement data from existing slides
        const allAnnouncementsData = allSlides.map((slide) => {
            const titleEl = slide.querySelector('.announcement-title');
            const contentEl = slide.querySelector('.announcement-content');
            const haberValue = slide.getAttribute('data-haber');
            const etkinlikValue = slide.getAttribute('data-etkinlik');
            const backgroundImage = slide.style.backgroundImage;
            
            return {
                haber: haberValue === 'true' || haberValue === 'True' || haberValue === true,
                etkinlik: etkinlikValue === 'true' || etkinlikValue === 'True' || etkinlikValue === true,
                title: titleEl ? titleEl.textContent.trim() : '',
                content: contentEl ? contentEl.textContent.trim() : '',
                posterImageUrl: backgroundImage ? backgroundImage.replace(/^url\(["']?|["']?\)$/g, '') : ''
            };
        });
    
    // Filter announcements by haber status and etkinlik - Keep them completely separate
    // News only (haber = true, etkinlik = false) - maintain original order (already sorted by ID from backend)
    const haberler = allAnnouncementsData.filter(x => x.haber === true && x.etkinlik !== true);
    
    // Announcements only (haber = false, etkinlik = false) - maintain original order (already sorted by ID from backend)
    const duyurular = allAnnouncementsData.filter(x => x.haber === false && x.etkinlik !== true);
    
    // Events only (etkinlik = true) - maintain original order (already sorted by ID from backend)
    const etkinlikler = allAnnouncementsData.filter(x => x.etkinlik === true);
    
    // Make arrays globally available for ticker to use
    window.announcementData = {
        duyurular: duyurular.filter(x => x.title && x.title.length > 0),
        haberler: haberler.filter(x => x.title && x.title.length > 0),
        etkinlikler: etkinlikler.filter(x => x.title && x.title.length > 0)
    };
    
    // Current mode state: 'haberler' → 'duyurular' → 'etkinlikler' → 'haberler' (cycle)
    // Start with haberler as requested
    let currentMode = 'haberler'; // Start with haberler
    let currentSlideIndex = 0;
    let slideInterval = null;
    let currentSlides = []; // For slide rotation only
    
    // Function to rebuild slider DOM with filtered items
    // mode = 'haberler' → shows ONLY news (haberler list)
    // mode = 'duyurular' → shows ONLY announcements (duyurular list)
    // mode = 'etkinlikler' → shows ONLY events (etkinlikler list)
    function updateSlider(mode) {
        // Update global mode state
        currentMode = mode;
        
        // Stop existing slide rotation
        if (slideInterval) {
            clearInterval(slideInterval);
            slideInterval = null;
        }
        
        // Get the appropriate list based on mode - STRICT SEPARATION
        let filteredItems = [];
        let emptyMessage = '';
        
        if (mode === 'haberler') {
            filteredItems = haberler;
            emptyMessage = 'Haber Bulunamadı';
        } else if (mode === 'duyurular') {
            filteredItems = duyurular;
            emptyMessage = 'Duyuru Bulunamadı';
        } else if (mode === 'etkinlikler') {
            filteredItems = etkinlikler;
            emptyMessage = 'Etkinlik Bulunamadı';
        }
        
        // Clear existing slides from DOM
        carousel.innerHTML = '';
        
        if (filteredItems.length === 0) {
            // Show empty state
            const emptySlide = document.createElement('div');
            emptySlide.className = 'announcement-slide active';
            emptySlide.innerHTML = `
                <div class="announcement-overlay">
                    <h3 class="announcement-title">${emptyMessage}</h3>
                </div>
            `;
            // Set attributes for empty state
            emptySlide.setAttribute('data-haber', mode === 'haberler' ? 'true' : 'false');
            emptySlide.setAttribute('data-etkinlik', mode === 'etkinlikler' ? 'true' : 'false');
            carousel.appendChild(emptySlide);
            currentSlides = [emptySlide];
        } else {
            // Rebuild slides dynamically
            currentSlides = filteredItems.map((item, index) => {
                const slide = document.createElement('div');
                slide.className = 'announcement-slide';
                if (index === 0) {
                    slide.classList.add('active');
                }
                
                const bgImage = item.posterImageUrl ? `background-image:url('${item.posterImageUrl}')` : '';
                slide.setAttribute('style', bgImage);
                slide.setAttribute('data-haber', item.haber.toString());
                slide.setAttribute('data-etkinlik', item.etkinlik ? 'true' : 'false');
                
                slide.innerHTML = `
                    <div class="announcement-overlay">
                        <h3 class="announcement-title">${item.title}</h3>
                        ${item.content ? `<p class="announcement-content">${item.content}</p>` : ''}
                    </div>
                `;
                
                carousel.appendChild(slide);
                return slide;
            });
        }
        
        // Reset slide index
        currentSlideIndex = 0;
        
        // Restart slide rotation - 10 seconds per slide
        slideInterval = setInterval(switchToNextSlide, 10000);
    }
    
    function switchToNextSlide() {
        if (currentSlides.length === 0) return;
        
        // Store previous index to detect when we complete a cycle
        const previousIndex = currentSlideIndex;
        
        // Remove active class from current slide
        currentSlides[currentSlideIndex].classList.remove('active');
        currentSlides[currentSlideIndex].classList.add('prev');
        
        // Move to next slide
        currentSlideIndex = (currentSlideIndex + 1) % currentSlides.length;
        
        // Check if we've completed a full cycle (went from last slide to first)
        // This happens when previousIndex was the last slide and now we're at 0
        if (previousIndex === currentSlides.length - 1 && currentSlideIndex === 0 && currentSlides.length > 1) {
            // We've completed one full cycle, switch to the next mode in rotation
            // Rotation: haberler → duyurular → etkinlikler → haberler
            const nextMode = getNextMode();
            if (hasItemsForMode(nextMode)) {
                // Switch to the next mode after animation
                setTimeout(() => {
                    updateSlider(nextMode);
                }, 500);
                return;
            }
        }
        
        // If only one slide in current list, check if we should switch
        if (currentSlides.length === 1) {
            const nextMode = getNextMode();
            if (hasItemsForMode(nextMode)) {
                // Switch to the next mode
                setTimeout(() => {
                    updateSlider(nextMode);
                }, 500);
                return;
            }
        }
        
        // Show next slide
        setTimeout(() => {
            currentSlides.forEach(slide => slide.classList.remove('prev'));
            currentSlides[currentSlideIndex].classList.add('active');
        }, 500);
    }
    
    // Get next mode in rotation: haberler → duyurular → etkinlikler → haberler
    function getNextMode() {
        if (currentMode === 'haberler') {
            return 'duyurular';
        } else if (currentMode === 'duyurular') {
            return 'etkinlikler';
        } else if (currentMode === 'etkinlikler') {
            return 'haberler';
        }
        return 'haberler'; // Default fallback
    }
    
    // Check if there are items for a given mode
    function hasItemsForMode(mode) {
        if (mode === 'haberler') {
            return haberler.length > 0;
        } else if (mode === 'duyurular') {
            return duyurular.length > 0;
        } else if (mode === 'etkinlikler') {
            return etkinlikler.length > 0;
        }
        return false;
    }
    
    // Initialize: Show haberler first as requested
    updateSlider('haberler');
}

// Independent Bottom Ticker - Fetches data from API and creates continuous flow
function initializeIndependentTicker() {
    const scrollingTextEl = document.querySelector('.scrolling-text');
    if (!scrollingTextEl) {
        return;
    }
    
    // Fetch data from API
    fetch('/Dashboard/GetScrollingAnnouncements')
        .then(res => res.json())
        .then(data => {
            // Admin özel bir kayan yazı girdiyse onu kullan
            if (data.customText && data.customText.length > 0) {
                const content = data.customText;
                let duplicated = '';
                for (let i = 0; i < 8; i++) {
                    duplicated += content + ' • ';
                }
                scrollingTextEl.innerHTML = duplicated;

                // Özel yazı kırmızı ve biraz daha belirgin olsun
                scrollingTextEl.style.color = '#ff4d4d';
                scrollingTextEl.style.fontWeight = '700';

                // Reset animation
                scrollingTextEl.style.animation = 'none';
                void scrollingTextEl.offsetHeight;
                // Custom text biraz daha hızlı aksın diye süreyi düşürüyoruz
                scrollingTextEl.style.animation = 'scroll-left 80s linear infinite';
                scrollingTextEl.style.animationPlayState = 'running';

                scrollingTextEl.addEventListener('mouseenter', function () {
                    this.style.animationPlayState = 'paused';
                });

                scrollingTextEl.addEventListener('mouseleave', function () {
                    this.style.animationPlayState = 'running';
                });
                return;
            }

            const haberler = (data.haberler || []).filter(x => x.title && x.title.length > 0);
            const duyurular = (data.duyurular || []).filter(x => x.title && x.title.length > 0);
            const etkinlikler = (data.etkinlikler || []).filter(x => x.title && x.title.length > 0);
            
            // Check if all lists are empty
            if (haberler.length === 0 && duyurular.length === 0 && etkinlikler.length === 0) {
                scrollingTextEl.innerHTML = 'Duyuru bulunamadı • Duyuru bulunamadı • Duyuru bulunamadı';
                return;
            }
            
            // Build continuous flow: Haberler -> Duyurular -> Etkinlikler -> Haberler...
            let combinedText = '';
            
            // Start with Haberler
            if (haberler.length > 0) {
                const haberTitles = haberler.map(item => item.title);
                combinedText += 'Haberler: ' + haberTitles.join(' • ');
            }
            
            // Then Duyurular
            if (duyurular.length > 0) {
                if (combinedText.length > 0) {
                    combinedText += ' • ';
                }
                const duyuruTitles = duyurular.map(item => item.title);
                combinedText += 'Duyurular: ' + duyuruTitles.join(' • ');
            }
            
            // Then Etkinlikler
            if (etkinlikler.length > 0) {
                if (combinedText.length > 0) {
                    combinedText += ' • ';
                }
                const etkinlikTitles = etkinlikler.map(item => item.title);
                combinedText += 'Etkinlikler: ' + etkinlikTitles.join(' • ');
            }
            
            // If only one type exists, just use that
            if (haberler.length === 0 && duyurular.length === 0 && etkinlikler.length > 0) {
                const etkinlikTitles = etkinlikler.map(item => item.title);
                combinedText = 'Etkinlikler: ' + etkinlikTitles.join(' • ');
            } else if (haberler.length === 0 && etkinlikler.length === 0 && duyurular.length > 0) {
                const duyuruTitles = duyurular.map(item => item.title);
                combinedText = 'Duyurular: ' + duyuruTitles.join(' • ');
            } else if (duyurular.length === 0 && etkinlikler.length === 0 && haberler.length > 0) {
                const haberTitles = haberler.map(item => item.title);
                combinedText = 'Haberler: ' + haberTitles.join(' • ');
            }
            
            // Create seamless loop by duplicating the content multiple times
            const duplicatedContent = combinedText + ' • ' + combinedText + ' • ' + combinedText + ' • ' + combinedText;
            scrollingTextEl.innerHTML = duplicatedContent;
            
            // Reset and restart animation to ensure it starts from the beginning
            scrollingTextEl.style.animation = 'none';
            // Force reflow to ensure style reset is applied
            void scrollingTextEl.offsetHeight;
            scrollingTextEl.style.animation = 'scroll-left 150s linear infinite';
            scrollingTextEl.style.animationPlayState = 'running';
            
            // Initialize hover pause functionality
            scrollingTextEl.addEventListener('mouseenter', function() {
                this.style.animationPlayState = 'paused';
            });
            
            scrollingTextEl.addEventListener('mouseleave', function() {
                this.style.animationPlayState = 'running';
            });
        })
        .catch(err => {
            console.error('Error fetching scrolling announcements:', err);
            scrollingTextEl.innerHTML = 'Duyuru bulunamadı • Duyuru bulunamadı • Duyuru bulunamadı';
        });
}

// Scrolling Text Animation - Only handles credits scrolling
function initializeScrollingText() {
    // Only initialize credits scrolling, announcements ticker is handled independently
    const scrollingCredits = document.querySelector('.scrolling-text-credits');
    
    if (scrollingCredits) {
        // Duplicate content many times for truly seamless continuous scrolling (like upper text)
        const content = scrollingCredits.textContent.trim();
        // Create continuous loop by duplicating content many times - ensure no gaps
        // Duplicate enough times so animation never shows a gap (same approach as upper text)
        // Create a very long string to ensure seamless scrolling
        let duplicatedContent = '';
        for (let i = 0; i < 20; i++) {
            duplicatedContent += content + ' • ';
        }
        scrollingCredits.innerHTML = duplicatedContent;
        
        // Reset and restart animation to ensure seamless loop
        scrollingCredits.style.animation = 'none';
        // Force reflow to ensure style reset is applied
        void scrollingCredits.offsetHeight;
        scrollingCredits.style.animation = 'scroll-left 80s linear infinite';
        scrollingCredits.style.animationPlayState = 'running';
        
        // Add hover pause functionality
        scrollingCredits.addEventListener('mouseenter', function() {
            this.style.animationPlayState = 'paused';
        });
        
        scrollingCredits.addEventListener('mouseleave', function() {
            this.style.animationPlayState = 'running';
        });
    }
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
