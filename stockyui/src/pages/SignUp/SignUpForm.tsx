import React from "react";
import TextField from "@mui/material/TextField";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Alert from "@mui/material/Alert";
import { useState } from "react";
import { AuthService } from "../../services/auth";

const API_BASE_URL = process.env.REACT_APP_API_BASE_URL;

interface SignUpSuccessResponse {
  message: string;
  token: string;
}

interface SignUpErrorResponse {
  message: string;
}

const SignUpForm = () => {
  const [firstName, setFirstName] = useState("");
  const [surname, setSurname] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    // call api here
    try {
      const response = await fetch(`${API_BASE_URL}/api/auth/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ firstName, surname, email, password }),
      });

      if (!response.ok) {
        const errorData = (await response.json()) as SignUpErrorResponse;
        setError(errorData.message || "Sign up failed");
        console.error("Sign up failed:", errorData);
        return;
      }

      const data = (await response.json()) as SignUpSuccessResponse;
      console.log("Sign up sucessful:", data.message);
      console.log("Sign up sucessful:", data.token);
      console.log(data.token);

      // setup auth
      AuthService.setToken(data.token);

      if (!AuthService.isAuthenticated()) {
        setError("Invalid or expired token recieved");
        return;
      }

      console.log("Signup successful:", data.message);
    } catch (err) {
      console.error("Signup error:", err);
      setError("An error occured. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Box component="form" onSubmit={handleSubmit} sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      <TextField
        label="First Name"
        value={firstName}
        onChange={(e) => setFirstName(e.target.value)}
        required
      />
      <TextField
        label="Surname"
        value={surname}
        onChange={(e) => setSurname(e.target.value)}
        required
      />
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
      <Button type="submit" variant="contained" color="primary">
      {isLoading ? "Signing up..." : "Signup"}
      </Button>
      {error && <Alert severity="error">{error}</Alert>}
    </Box>
  );
};

export default SignUpForm;