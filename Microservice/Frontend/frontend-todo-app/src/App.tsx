import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Login from './components/Login';
import Register from './components/Register';
import Dashboard from './components/Dashboard';
import ProtectedRoute from './components/ProtectedRoute';
import TestTailwind from './test-tailwind';

function App() {
  // Debug: Check localStorage on app load
  console.log('App.tsx: localStorage accessToken exists?', !!localStorage.getItem('accessToken'));
  console.log('App.tsx: localStorage refreshToken exists?', !!localStorage.getItem('refreshToken'));

  return (
    <Router>
      <AuthProvider>
        <Routes>
          <Route path="/test" element={<TestTailwind />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route path="/" element={<Navigate to="/login" replace />} />
        </Routes>
      </AuthProvider>
    </Router>
  );
}

export default App
