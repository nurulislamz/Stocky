import * as React from 'react';
import Modal from '@mui/material/Modal';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import Paper from '@mui/material/Paper';
import Divider from '@mui/material/Divider';
import IconButton from '@mui/material/IconButton';
import CloseIcon from '@mui/icons-material/Close';
import AccountBalanceWalletIcon from '@mui/icons-material/AccountBalanceWallet';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import { usePortfolio } from '../hooks/usePortfolio';

interface AddFundsModalProps {
  open: boolean;
  onClose: () => void;
}

export default function AddFundsModal({ open, onClose }: AddFundsModalProps) {
  const [amount, setAmount] = React.useState('');
  const [isSubmitting, setIsSubmitting] = React.useState(false);
  const [error, setError] = React.useState('');

  const { addFunds } = usePortfolio();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError('');

    try {
      const amountNum = parseFloat(amount);

      if (!amountNum || amountNum <= 0) {
        setError('Please enter a valid amount');
        return;
      }

      const result = await addFunds(amountNum);

      if (result.success) {
        console.log(`Successfully added £${amount} to portfolio`);
        onClose();
        setAmount('');
      } else {
        setError(result.message || 'Failed to add funds');
      }
    } catch (error) {
      setError('An unexpected error occurred');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Modal
      open={open}
      onClose={onClose}
      aria-labelledby="add-funds-modal-title"
    >
      <Box sx={{
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: { xs: '90%', sm: 400 },
        maxWidth: 400,
        outline: 'none',
      }}>
        <Paper
          elevation={24}
          sx={{
            borderRadius: 3,
            overflow: 'hidden',
            background: 'white',
          }}
        >
          {/* Header */}
          <Box sx={{
            p: 3,
            pb: 2,
            background: 'linear-gradient(135deg, #2c3e50 0%, #34495e 100%)',
          }}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                <AccountBalanceWalletIcon sx={{ color: 'white', fontSize: 28 }} />
                <Typography
                  variant="h5"
                  component="h2"
                  sx={{
                    color: 'white',
                    fontWeight: 700,
                    fontSize: { xs: '1.5rem', sm: '1.75rem' }
                  }}
                >
                  Add Funds
                </Typography>
              </Box>
              <IconButton
                onClick={onClose}
                sx={{
                  color: 'white',
                  '&:hover': {
                    backgroundColor: 'rgba(255, 255, 255, 0.1)'
                  }
                }}
              >
                <CloseIcon />
              </IconButton>
            </Box>
            <Typography variant="body2" sx={{ color: 'rgba(255, 255, 255, 0.8)' }}>
              Add money to your portfolio balance
            </Typography>
          </Box>

          {/* Content */}
          <Box sx={{ p: 3, backgroundColor: 'white' }}>
            <form onSubmit={handleSubmit} noValidate>
              <Stack spacing={3}>
                <TextField
                  label="Amount"
                  type="number"
                  value={amount}
                  onChange={(e) => setAmount(e.target.value)}
                  required
                  fullWidth
                  variant="outlined"
                  placeholder="0.00"
                  InputProps={{
                    startAdornment: <AttachMoneyIcon sx={{ mr: 1, ml: 1, color: 'text.secondary' }} />,
                    inputProps: {
                      min: 0.01,
                      step: 0.01
                    },
                    sx: { pl: 0 }
                  }}
                  sx={{
                    '& .MuiOutlinedInput-root': {
                      borderRadius: 2,
                      '&:hover fieldset': {
                        borderColor: '#667eea',
                      },
                      '&.Mui-focused fieldset': {
                        borderColor: '#667eea',
                      },
                    },
                    '& .MuiInputLabel-root': {
                      backgroundColor: 'white',
                      px: 1,
                    },
                  }}
                />

                {/* Error Display */}
                {error && (
                  <Typography variant="body2" sx={{ color: 'error.main', textAlign: 'center' }}>
                    {error}
                  </Typography>
                )}

                {/* Amount Display */}
                {amount && (
                  <Paper
                    elevation={0}
                    sx={{
                      p: 2,
                      backgroundColor: '#f8f9fa',
                      borderRadius: 2,
                      border: '1px solid #e9ecef',
                    }}
                  >
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                      <Typography variant="body1" sx={{ fontWeight: 600, color: 'text.secondary' }}>
                        Amount to Add:
                      </Typography>
                      <Typography variant="h6" sx={{ fontWeight: 700, color: '#667eea' }}>
                        £{parseFloat(amount) ? parseFloat(amount).toFixed(2) : '0.00'}
                      </Typography>
                    </Box>
                  </Paper>
                )}

                <Divider sx={{ my: 1 }} />

                {/* Action Buttons */}
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                  <Button
                    variant="contained"
                    type="submit"
                    fullWidth
                    size="large"
                    disabled={isSubmitting}
                    sx={{
                      backgroundColor: '#4caf50',
                      borderRadius: 2,
                      py: { xs: 2, sm: 1.5 },
                      fontWeight: 600,
                      textTransform: 'none',
                      fontSize: { xs: '1.1rem', sm: '1rem' },
                      '&:hover': {
                        backgroundColor: '#45a049',
                        transform: 'translateY(-1px)',
                        boxShadow: '0 4px 12px rgba(76, 175, 80, 0.3)',
                      },
                      transition: 'all 0.2s ease-in-out',
                    }}
                  >
                    {isSubmitting ? 'Adding Funds...' : 'Add Funds'}
                  </Button>
                  <Button
                    variant="outlined"
                    onClick={onClose}
                    fullWidth
                    size="large"
                    disabled={isSubmitting}
                    sx={{
                      borderRadius: 2,
                      py: { xs: 2, sm: 1.5 },
                      fontWeight: 600,
                      textTransform: 'none',
                      fontSize: { xs: '1.1rem', sm: '1rem' },
                      borderColor: '#667eea',
                      color: '#667eea',
                      '&:hover': {
                        borderColor: '#5a6fd8',
                        backgroundColor: 'rgba(102, 126, 234, 0.04)',
                      },
                    }}
                  >
                    Cancel
                  </Button>
                </Stack>
              </Stack>
            </form>
          </Box>
        </Paper>
      </Box>
    </Modal>
  );
}