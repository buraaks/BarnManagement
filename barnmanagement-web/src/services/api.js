import axios from "axios";

const apiClient = axios.create({
  baseURL: "https://localhost:7067/api",
  headers: {
    "Content-Type": "application/json",
  },
});

export default {
  // Auth
  login(credentials) {
    return apiClient.post("/auth/login", credentials);
  },

  // Animals
  getAnimals() {
    return apiClient.get("/animals");
  },

  // Farms
  getFarms() {
    return apiClient.get("/farms");
  },

  // Health check
  checkHealth() {
    return apiClient.get("https://localhost:7067/health");
  },
};
