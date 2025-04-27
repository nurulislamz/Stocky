import React from "react";
import TextField from "@mui/material/TextField";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Alert from "@mui/material/Alert";
import { useState } from "react";
import { AuthService } from "../../services/auth";

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

interface LoginSuccessResponse {
  message: string;
  token: string;
}

interface LoginErrorResponse {
  message: string;
}

const LoginForm = () => {
  const [email, setEmail] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    // call api here
    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password })
      });

      if (!response.ok) {
        const errorData = await response.json() as LoginErrorResponse;
        setError(errorData.message || "Login failed");
        console.error("Login failed:", errorData);
        return;
      }

      const data = await response.json() as LoginSuccessResponse;
      console.log("Login sucessful:", data.message);
      console.log("Login sucessful:", data.token);
      console.log(data.token);

      // setup auth
      AuthService.setToken(data.token);

      if (!AuthService.isAuthenticated()) {
        setError("Invalid or expired token recieved");
        return;
      }

      console.log("Login successful:", data.message);
    }
    catch (err) {
      console.error("Login error:", err);
      setError("An error occured. Please try again.");
    }
    finally {
      setIsLoading(false);
    }
  }

  return (
    <Box
      component="form"
      onSubmit={handleSubmit}
      sx={{ display: "flex", flexDirection: "column", gap: 2 }}
    >
      <TextField
        label="Email"
        value={email}
        onChange={(e) => {
          setEmail(e.target.value);
          setError(null); // Clear error on change
        }}
        required
      />
      <TextField
        label="Password"
        type="password"
        value={password}
        onChange={(e) => {
          setPassword(e.target.value);
          setError(null); // Clear error on change
        }}
        required
      />
      <Button
        type="submit"
        variant="contained"
        color="primary"
        disabled={isLoading}
      >
        {isLoading ? "Logging in..." : "Login"}
      </Button>
      {error && <Alert severity="error">{error}</Alert>}
    </Box>
  );
};

export default LoginForm;