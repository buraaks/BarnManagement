<script setup>
import { ref, onMounted } from 'vue';
import api from './services/api';

const healthStatus = ref('loading');
const animalsCount = ref(0);
const farmsCount = ref(0);

onMounted(async () => {
  try {
    // Check API Health
    // Note: Since the backend might not be running right now, we handle the error gracefully
    const health = await api.checkHealth().catch(() => ({ status: 500 }));
    healthStatus.value = health.status === 200 ? 'online' : 'offline';
    
    // In a real app, we would fetch data here
    animalsCount.value = 124; // Mock for design
    farmsCount.value = 12;    // Mock for design
  } catch (error) {
    healthStatus.value = 'offline';
  }
});
</script>

<template>
  <div class="dashboard">
    <nav class="glass nav">
      <div class="logo">
        <span class="icon">🐄</span>
        <h1>Barn<span>Management</span></h1>
      </div>
      <div class="nav-links">
        <a href="#">Dashboard</a>
        <a href="#">Animals</a>
        <a href="#">Stats</a>
        <div class="status-badge" :class="healthStatus">
          {{ healthStatus === 'online' ? 'API Online' : 'API Connecting...' }}
        </div>
      </div>
    </nav>

    <main class="container">
      <header class="hero animate-fade-up">
        <h2>Welcome to Your Modern Barn</h2>
        <p>Intelligent management for your livestock and production cycles.</p>
      </header>

      <div class="stats-grid">
        <div class="stat-card glass animate-fade-up" style="animation-delay: 0.1s">
          <div class="stat-value">{{ animalsCount }}</div>
          <div class="stat-label">Total Animals</div>
          <div class="stat-trend">+5% this week</div>
        </div>
        
        <div class="stat-card glass animate-fade-up" style="animation-delay: 0.2s">
          <div class="stat-value">{{ farmsCount }}</div>
          <div class="stat-label">Active Barns</div>
          <div class="stat-trend">Full capacity</div>
        </div>

        <div class="stat-card glass animate-fade-up" style="animation-delay: 0.3s">
          <div class="stat-value">98%</div>
          <div class="stat-label">Health Score</div>
          <div class="stat-trend">Excellent</div>
        </div>
      </div>

      <section class="actions-section animate-fade-up" style="animation-delay: 0.4s">
        <div class="glass action-card">
          <h3>Quick Links</h3>
          <div class="btn-group">
            <button class="btn btn-primary">Add New Animal</button>
            <button class="btn glass">Export Reports</button>
          </div>
        </div>
      </section>
    </main>

    <footer class="footer">
      <p>&copy; 2026 Barn Management System. Premium Agriculture Solutions.</p>
    </footer>
  </div>
</template>

<style scoped>
.dashboard {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.nav {
  margin: 1.5rem;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: sticky;
  top: 1.5rem;
  z-index: 100;
}

.logo {
  display: flex;
  align-items: center;
  gap: 12px;
}

.logo h1 {
  font-size: 1.4rem;
}

.logo span {
  background: linear-gradient(45deg, var(--accent), var(--secondary));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.nav-links a {
  color: var(--text-muted);
  text-decoration: none;
  font-weight: 500;
  transition: var(--transition);
}

.nav-links a:hover {
  color: var(--text);
}

.status-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
  background: rgba(255, 255, 255, 0.1);
}

.status-badge.online {
  color: #4ade80;
  box-shadow: 0 0 10px rgba(74, 222, 128, 0.2);
}

.status-badge.offline {
  color: #fb7185;
}

.hero {
  text-align: center;
  margin-top: 4rem;
  margin-bottom: 3rem;
}

.hero h2 {
  font-size: 3rem;
  margin-bottom: 1rem;
  background: linear-gradient(to right, #fff, var(--text-muted));
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.hero p {
  color: var(--text-muted);
  font-size: 1.2rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.stat-card {
  padding: 2rem;
  text-align: center;
  transition: var(--transition);
}

.stat-card:hover {
  border-color: var(--primary);
  transform: translateY(-5px);
}

.stat-value {
  font-size: 2.5rem;
  font-weight: 800;
  margin-bottom: 0.5rem;
  color: var(--text);
}

.stat-label {
  color: var(--text-muted);
  text-transform: uppercase;
  letter-spacing: 1.5px;
  font-size: 0.9rem;
  margin-bottom: 1rem;
}

.stat-trend {
  font-size: 0.8rem;
  color: #4ade80;
}

.action-card {
  padding: 2.5rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
}

.btn-group {
  display: flex;
  gap: 1rem;
}

.footer {
  margin-top: auto;
  padding: 2rem;
  text-align: center;
  color: var(--text-muted);
  font-size: 0.9rem;
}
</style>
