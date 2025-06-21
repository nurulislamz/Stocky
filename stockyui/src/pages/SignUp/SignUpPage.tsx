import { Navigate } from 'react-router-dom';
import React, { useMemo, useState} from 'react';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Checkbox from '@mui/material/Checkbox';
import CssBaseline from '@mui/material/CssBaseline';
import Divider from '@mui/material/Divider';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormLabel from '@mui/material/FormLabel';
import FormControl from '@mui/material/FormControl';
import Link from '@mui/material/Link';
import TextField from '@mui/material/TextField';
import Typography from '@mui/material/Typography';
import Alert from '@mui/material/Alert';
import CircularProgress from '@mui/material/CircularProgress';
import AppTheme from '../../shared-theme/AppTheme';
import ColorModeSelect from '../../shared-theme/ColorModeSelect';
import { GoogleIcon, FacebookIcon, SitemarkIcon } from '../../components/CustomIcons';
import Card from '../../components/Card';
import StackContainer from '../../components/StackContainer';
import { AuthService } from "../../services/auth.service";
import { StockyApi } from '../../services/generated/stockyapi';
import { useAuth } from '../../hooks/useAuth';

interface FormData {
  firstName: string;
  surname: string;
  email: string;
  password: string;
}

interface FormErrors {
  firstName?: string;
  surname?: string;
  email?: string;
  password?: string;
  server?: string;
}

export default function SignUp(props: { disableCustomTheme?: boolean }) {

  const authService = useMemo(() => new AuthService(), []);

  const { isAuthenticated } = useAuth();
  const [formData, setFormData] = useState<FormData>({
    firstName: '',
    surname: '',
    email: '',
    password: ''
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [isLoading, setIsLoading] = useState<boolean>(false);

  if (isAuthenticated) {
    return <Navigate to="/home" replace />
  }

  const validateInputs = () => {
    const newErrors: FormErrors = {};

    if (!formData.firstName) {
      newErrors.firstName = 'First name is required';
    }

    if (!formData.surname) {
      newErrors.surname = 'Surname is required';
    }

    if (!formData.email) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Please enter a valid email address.';
    }

    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters long.';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setIsLoading(true);
    setErrors({});

    if (!validateInputs()) {
      setIsLoading(false);
      return;
    }

    try {
      const response = await authService.register(new StockyApi.RegisterRequest(formData));

      if (!response.success) {
        setErrors({ server: response.message || "Sign up failed" });
        return;
      }

      if (!authService.isAuthenticated()) {
        setErrors({ server: "Invalid or expired token received" });
        return;
      }

      console.log("Signup successful:", response.message);
    } catch (err) {
      console.error("Signup error:", err);
      setErrors({ server: "Unknown error occurred. Please try again." });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <AppTheme {...props}>
      <CssBaseline enableColorScheme />
      <ColorModeSelect sx={{ position: 'fixed', top: '1rem', right: '1rem' }} />
      <StackContainer direction="column" justifyContent="space-between">
        <Card variant="outlined">
          <SitemarkIcon />
          <Typography
            component="h1"
            variant="h4"
            sx={{ width: '100%', fontSize: 'clamp(2rem, 10vw, 2.15rem)' }}
          >
            Sign up
          </Typography>
          {errors.firstName && (
            <Alert severity="error" sx={{ width: '100%', mb: 2 }}>
              {errors.firstName}
            </Alert>
          )}
          {errors.surname && (
            <Alert severity="error" sx={{ width: '100%', mb: 2 }}>
              {errors.surname}
            </Alert>
          )}
          {errors.email && (
            <Alert severity="error" sx={{ width: '100%', mb: 2 }}>
              {errors.email}
            </Alert>
          )}
          {errors.password && (
            <Alert severity="error" sx={{ width: '100%', mb: 2 }}>
              {errors.password}
            </Alert>
          )}
          {errors.server && (
            <Alert severity="error" sx={{ width: '100%', mb: 2 }}>
              {errors.server}
            </Alert>
          )}
          <Box
            component="form"
            onSubmit={handleSubmit}
            sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
          >
            <FormControl>
              <FormLabel htmlFor="firstname">First name</FormLabel>
              <TextField
                autoComplete="firstname"
                name="firstname"
                required
                fullWidth
                id="firstname"
                placeholder="Jon"
                error={!!errors.firstName}
                helperText={errors.firstName}
                color={!!errors.firstName ? 'error' : 'primary'}
                onChange={(e) => {
                  setFormData({ ...formData, firstName: e.target.value });
                  setErrors({ ...errors, firstName: undefined });
                }}
              />
            </FormControl>
            <FormControl>
              <FormLabel htmlFor="surname">Surname</FormLabel>
              <TextField
                autoComplete="surname"
                name="surname"
                required
                fullWidth
                id="surname"
                placeholder="Snow"
                error={!!errors.surname}
                helperText={errors.surname}
                color={!!errors.surname ? 'error' : 'primary'}
                onChange={(e) => {
                  setFormData({ ...formData, surname: e.target.value });
                  setErrors({ ...errors, surname: undefined });
                }}
              />
            </FormControl>
            <FormControl>
              <FormLabel htmlFor="email">Email</FormLabel>
              <TextField
                required
                fullWidth
                id="email"
                placeholder="your@email.com"
                name="email"
                autoComplete="email"
                variant="outlined"
                error={!!errors.email}
                helperText={errors.email}
                color={!!errors.email ? 'error' : 'primary'}
                onChange={(e) => {
                  setFormData({ ...formData, email: e.target.value });
                  setErrors({ ...errors, email: undefined });
                }}
              />
            </FormControl>
            <FormControl>
              <FormLabel htmlFor="password">Password</FormLabel>
              <TextField
                required
                fullWidth
                name="password"
                placeholder="••••••"
                type="password"
                id="password"
                autoComplete="new-password"
                variant="outlined"
                error={!!errors.password}
                helperText={errors.password}
                color={!!errors.password ? 'error' : 'primary'}
                onChange={(e) => {
                  setFormData({ ...formData, password: e.target.value });
                  setErrors({ ...errors, password: undefined });
                }}
              />
            </FormControl>
            <FormControlLabel
              control={<Checkbox value="allowExtraEmails" color="primary" />}
              label="I want to receive updates via email."
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              disabled={isLoading}
              startIcon={isLoading ? <CircularProgress size={20} sx={{ color: 'white' }} /> : null}
            >
              {isLoading ? 'Signing up...' : 'Sign up'}
            </Button>
          </Box>
          <Divider>
            <Typography sx={{ color: 'text.secondary' }}>or</Typography>
          </Divider>
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <Button
              fullWidth
              variant="outlined"
              onClick={() => alert('Sign up with Google')}
              startIcon={<GoogleIcon />}
            >
              Sign up with Google
            </Button>
            <Button
              fullWidth
              variant="outlined"
              onClick={() => alert('Sign up with Facebook')}
              startIcon={<FacebookIcon />}
            >
              Sign up with Facebook
            </Button>
            <Typography sx={{ textAlign: 'center' }}>
              Already have an account?{' '}
              <Link href="./login" variant="body2">
                Sign in
              </Link>
            </Typography>
          </Box>
        </Card>
      </StackContainer>
    </AppTheme>
  );
}