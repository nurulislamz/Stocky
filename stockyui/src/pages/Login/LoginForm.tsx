import React from "react";
import TextField from "@mui/material/TextField";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import { useState } from "react";

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

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
        const errorData = await response.json();
        setError(errorData.message || "Login failed");
        console.error("Login failed:", errorData);
        return;
      }

      const data = await response.json();
      console.log("Login sucessful:", data);
      console.log(data.token);
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
    <Box component="form" onSubmit={handleSubmit} sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      <TextField
        label="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      <TextField
        label="Password"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
      <Button type="submit" variant="contained" color="primary" disabled={isLoading}>
        {isLoading ? "Logging in..." : "Login"}
      </Button>
    </Box>
  );
};

export default LoginForm;