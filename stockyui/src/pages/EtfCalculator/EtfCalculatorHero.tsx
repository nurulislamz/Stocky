import { useState } from 'react';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import FormControl from '@mui/material/FormControl';
import FormLabel from '@mui/material/FormLabel';
import TextField from '@mui/material/TextField';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Grid from '@mui/material/Grid';
import Alert from '@mui/material/Alert';
import { useNavigate } from 'react-router-dom';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import AccountBalanceIcon from '@mui/icons-material/AccountBalance';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';

interface FormData {
  underlyingStock: string;
  etfSymbol: string;
  leverageMultiplier: string;
  currentPrice: string;
}

interface FormErrors {
  underlyingStock?: string;
  etfSymbol?: string;
  leverageMultiplier?: string;
  currentPrice?: string;
  server?: string;
}

export default function EtfCalculatorHero() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<FormData>({
    underlyingStock: '',
    etfSymbol: '',
    leverageMultiplier: '',
    currentPrice: ''
  });
  const [errors, setErrors] = useState<FormErrors>({});
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const leverageOptions = [
    { value: '-3', label: '-3x (Inverse 3x)' },
    { value: '-2', label: '-2x (Inverse 2x)' },
    { value: '-1', label: '-1x (Inverse)' },
    { value: '1', label: '1x (Standard)' },
    { value: '2', label: '2x (Leveraged)' },
    { value: '3', label: '3x (Leveraged)' }
  ];

  const validateInputs = () => {
    const newErrors: FormErrors = {};

    if (!formData.underlyingStock.trim()) {
      newErrors.underlyingStock = 'Underlying stock symbol is required';
    }

    if (!formData.etfSymbol.trim()) {
      newErrors.etfSymbol = 'ETF symbol is required';
    }

    if (!formData.leverageMultiplier) {
      newErrors.leverageMultiplier = 'Leverage multiplier is required';
    }

    if (!formData.currentPrice.trim()) {
      newErrors.currentPrice = 'Current price is required';
    } else if (isNaN(parseFloat(formData.currentPrice)) || parseFloat(formData.currentPrice) <= 0) {
      newErrors.currentPrice = 'Please enter a valid price greater than 0';
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
      const params = new URLSearchParams({
        underlying: formData.underlyingStock.trim().toUpperCase(),
        etf: formData.etfSymbol.trim().toUpperCase(),
        leverage: formData.leverageMultiplier,
        price: formData.currentPrice.trim()
      });

      navigate(`/etf-calculator?${params.toString()}`);
    } catch (err) {
      console.error('Navigation error:', err);
      setErrors({ server: 'An error occurred. Please try again.' });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Box sx={{
      width: "100%",
      maxWidth: { sm: "100%", md: "1700px" },
      height: { xs: 'auto', sm: 'auto', md: '100%' },
      overflow: { xs: 'auto', sm: 'auto', md: 'visible' }
    }}>
      <Typography
        component="h2"
        variant="h4"
        sx={{
          mb: 3,
          fontSize: {
            xs: '24px',
            sm: '28px',
            md: '32px',
            lg: '36px'
          }
        }}
      >
        ETF Calculator
      </Typography>

      <Typography
        variant="h6"
        color="text.secondary"
        sx={{
          mb: 4,
          fontSize: {
            xs: '16px',
            sm: '18px',
            md: '20px',
            lg: '22px'
          }
        }}
      >
        Compare leveraged ETF performance against underlying assets
      </Typography>

      {errors.server && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {errors.server}
        </Alert>
      )}

      <Paper sx={{
        p: { xs: 2, sm: 3, md: 4 },
        borderRadius: 2,
        backgroundColor: 'background.default',
        height: 'auto',
        overflow: 'visible'
      }}>
        <Box
          component="form"
          onSubmit={handleSubmit}
          noValidate
          sx={{
            height: '100%',
            overflow: 'visible'
          }}
        >
          <Grid
            container
            spacing={{ xs: 2, sm: 3, md: 4 }}
            sx={{
              height: '100%',
              overflow: 'visible',
              flexDirection: { xs: 'column', sm: 'column', md: 'column', lg: 'row' },
              flexWrap: { xs: 'nowrap', sm: 'nowrap', md: 'nowrap', lg: 'wrap' }
            }}
          >
            <Grid size={{ xs: 12, sm: 12, md: 12, lg: 6 }}>
              <FormControl fullWidth>
                <FormLabel
                  htmlFor="underlyingStock"
                  sx={{
                    mb: { xs: 1, sm: 1.5, md: 2 },
                    fontSize: {
                      xs: '14px',
                      sm: '16px',
                      md: '18px',
                      lg: '20px'
                    },
                    fontWeight: 500
                  }}
                >
                  Underlying Stock Symbol
                </FormLabel>
                <TextField
                  id="underlyingStock"
                  name="underlyingStock"
                  placeholder="e.g., SPY, QQQ, AAPL"
                  value={formData.underlyingStock}
                  onChange={(e) => {
                    setFormData({ ...formData, underlyingStock: e.target.value.toUpperCase() });
                    setErrors({ ...errors, underlyingStock: undefined });
                  }}
                  required
                  fullWidth
                  variant="outlined"
                  error={!!errors.underlyingStock}
                  helperText={errors.underlyingStock}
                  InputProps={{
                    startAdornment: (
                      <TrendingUpIcon
                        sx={{
                          mr: 1,
                          ml: 1,
                          color: 'text.secondary',
                          fontSize: { xs: '20px', sm: '24px', md: '28px' }
                        }}
                      />
                    ),
                    sx: {
                      height: { xs: '48px', sm: '56px', md: '64px' },
                      fontSize: { xs: '16px', sm: '18px', md: '20px' }
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2
                    },
                    '& .MuiFormHelperText-root': {
                      fontSize: { xs: '12px', sm: '14px', md: '16px' },
                      mt: 1
                    }
                  }}
                />
              </FormControl>
            </Grid>

            <Grid size={{ xs: 12, sm: 12, md: 12, lg: 6 }}>
              <FormControl fullWidth>
                <FormLabel
                  htmlFor="etfSymbol"
                  sx={{
                    mb: { xs: 1, sm: 1.5, md: 2 },
                    fontSize: {
                      xs: '14px',
                      sm: '16px',
                      md: '18px',
                      lg: '20px'
                    },
                    fontWeight: 500
                  }}
                >
                  Leveraged ETF Symbol
                </FormLabel>
                <TextField
                  id="etfSymbol"
                  name="etfSymbol"
                  placeholder="e.g., SPXL, TQQQ, TECL"
                  value={formData.etfSymbol}
                  onChange={(e) => {
                    setFormData({ ...formData, etfSymbol: e.target.value.toUpperCase() });
                    setErrors({ ...errors, etfSymbol: undefined });
                  }}
                  required
                  fullWidth
                  variant="outlined"
                  error={!!errors.etfSymbol}
                  helperText={errors.etfSymbol}
                  InputProps={{
                    startAdornment: (
                      <AccountBalanceIcon
                        sx={{
                          mr: 1,
                          ml: 1,
                          color: 'text.secondary',
                          fontSize: { xs: '20px', sm: '24px', md: '28px' }
                        }}
                      />
                    ),
                    sx: {
                      height: { xs: '48px', sm: '56px', md: '64px' },
                      fontSize: { xs: '16px', sm: '18px', md: '20px' }
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2
                    },
                    '& .MuiFormHelperText-root': {
                      fontSize: { xs: '12px', sm: '14px', md: '16px' },
                      mt: 1
                    }
                  }}
                />
              </FormControl>
            </Grid>

            <Grid size={{ xs: 12, sm: 12, md: 12, lg: 6 }}>
              <FormControl fullWidth>
                <FormLabel
                  htmlFor="leverageMultiplier"
                  sx={{
                    mb: { xs: 1, sm: 1.5, md: 2 },
                    fontSize: {
                      xs: '14px',
                      sm: '16px',
                      md: '18px',
                      lg: '20px'
                    },
                    fontWeight: 500
                  }}
                >
                  Leverage Multiplier
                </FormLabel>
                <Select
                  id="leverageMultiplier"
                  name="leverageMultiplier"
                  value={formData.leverageMultiplier}
                  onChange={(e) => {
                    setFormData({ ...formData, leverageMultiplier: e.target.value });
                    setErrors({ ...errors, leverageMultiplier: undefined });
                  }}
                  required
                  fullWidth
                  displayEmpty
                  error={!!errors.leverageMultiplier}
                  sx={{
                    borderRadius: 2,
                    height: { xs: '48px', sm: '56px', md: '64px' },
                    fontSize: { xs: '16px', sm: '18px', md: '20px' },
                    '& .MuiSelect-select': {
                      display: 'flex',
                      alignItems: 'center'
                    }
                  }}
                >
                  <MenuItem
                    value=""
                    disabled
                    sx={{
                      fontSize: { xs: '16px', sm: '18px', md: '20px' }
                    }}
                  >
                    Select leverage multiplier
                  </MenuItem>
                  {leverageOptions.map((option) => (
                    <MenuItem
                      key={option.value}
                      value={option.value}
                      sx={{
                        fontSize: { xs: '16px', sm: '18px', md: '20px' }
                      }}
                    >
                      {option.label}
                    </MenuItem>
                  ))}
                </Select>
                {errors.leverageMultiplier && (
                  <Typography
                    variant="caption"
                    color="error"
                    sx={{
                      mt: 1,
                      ml: 1.5,
                      fontSize: { xs: '12px', sm: '14px', md: '16px' }
                    }}
                  >
                    {errors.leverageMultiplier}
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid size={{ xs: 12, sm: 12, md: 12, lg: 6 }}>
              <FormControl fullWidth>
                <FormLabel
                  htmlFor="currentPrice"
                  sx={{
                    mb: { xs: 1, sm: 1.5, md: 2 },
                    fontSize: {
                      xs: '14px',
                      sm: '16px',
                      md: '18px',
                      lg: '20px'
                    },
                    fontWeight: 500
                  }}
                >
                  Current Average Price
                </FormLabel>
                <TextField
                  id="currentPrice"
                  name="currentPrice"
                  type="number"
                  placeholder="0.00"
                  value={formData.currentPrice}
                  onChange={(e) => {
                    setFormData({ ...formData, currentPrice: e.target.value });
                    setErrors({ ...errors, currentPrice: undefined });
                  }}
                  required
                  fullWidth
                  variant="outlined"
                  error={!!errors.currentPrice}
                  helperText={errors.currentPrice}
                  InputProps={{
                    startAdornment: (
                      <AttachMoneyIcon
                        sx={{
                          mr: 1,
                          ml: 1,
                          color: 'text.secondary',
                          fontSize: { xs: '20px', sm: '24px', md: '28px' }
                        }}
                      />
                    ),
                    inputProps: {
                      min: 0,
                      step: 0.01
                    },
                    sx: {
                      height: { xs: '48px', sm: '56px', md: '64px' },
                      fontSize: { xs: '16px', sm: '18px', md: '20px' }
                    }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2
                    },
                    '& .MuiFormHelperText-root': {
                      fontSize: { xs: '12px', sm: '14px', md: '16px' },
                      mt: 1
                    }
                  }}
                />
              </FormControl>
            </Grid>
          </Grid>

          <Box sx={{
            mt: { xs: 4, sm: 5, md: 6 },
            display: 'flex',
            justifyContent: 'center',
            pb: { xs: 2, sm: 2, md: 0 } // Add padding at bottom for mobile
          }}>
            <Button
              type="submit"
              variant="contained"
              disabled={isLoading}
              size="large"
              sx={{
                height: { xs: '48px', sm: '56px', md: '64px' },
                px: { xs: 4, sm: 5, md: 6 },
                borderRadius: 2,
                fontWeight: 600,
                textTransform: 'none',
                fontSize: { xs: '16px', sm: '18px', md: '20px' },
                minWidth: { xs: '200px', sm: '250px', md: '300px' },
                '&:hover': {
                  transform: 'translateY(-1px)',
                  boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
                },
                transition: 'all 0.2s ease-in-out',
              }}
            >
              {isLoading ? 'Calculating...' : 'Calculate Performance'}
            </Button>
          </Box>
        </Box>
      </Paper>
    </Box>
  );
}
