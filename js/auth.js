const API_URL = '/api';
let isLogin = true;

const authForm = document.getElementById('authForm');
const authTitle = document.getElementById('authTitle');
const authBtn = document.getElementById('authBtn');
const switchBtn = document.getElementById('switchBtn');
const switchText = document.getElementById('switchText');
const usernameGroup = document.getElementById('usernameGroup');

switchBtn.onclick = () => {
    isLogin = !isLogin;
    authTitle.innerText = isLogin ? 'Login' : 'Register';
    authBtn.innerText = isLogin ? 'Login' : 'Register';
    switchText.innerText = isLogin ? "Don't have an account? " : "Already have an account? ";
    switchBtn.innerText = isLogin ? 'Register' : 'Login';
    usernameGroup.style.display = isLogin ? 'none' : 'block';
};

authForm.onsubmit = async (e) => {
    e.preventDefault();
    
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const username = document.getElementById('username').value;

    const endpoint = isLogin ? '/auth/login' : '/auth/register';
    const body = isLogin ? { email, password } : { username, email, password };

    try {
        const response = await fetch(`${API_URL}${endpoint}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });

        const data = await response.json();

        if (response.ok) {
            localStorage.setItem('token', data.token);
            window.location.href = 'index.html';
        } else {
            alert(data.message || 'Authentication failed');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred during authentication');
    }
};
