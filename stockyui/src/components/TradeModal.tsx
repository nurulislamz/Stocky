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
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import NumbersIcon from '@mui/icons-material/Numbers';

interface TradeModalProps {
  open: boolean;
  onClose: () => void;
  symbol: string;
  price: string;
  type: 'buy' | 'sell';
}

export default function TradeModal({ open, onClose, symbol, type }: TradeModalProps) {
  const [ticker, setTicker] = React.useState(symbol);
  const [price, setPrice] = React.useState('');
  const [quantity, setQuantity] = React.useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // TODO: Implement trade logic
    console.log(`${type.toUpperCase()} ${quantity} shares of ${ticker} at ${price}`);
    onClose();
  };

  const total = quantity && price ? (parseFloat(price) * parseInt(quantity)).toFixed(2) : '0.00';

  return (
    <Modal
      open={open}
      onClose={onClose}
      aria-labelledby="trade-modal-title"
    >
      <Box sx={{
        position: 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: { xs: '90%', sm: 500 },
        maxWidth: 500,
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
                  {type === 'buy' ? 'Buy' : 'Sell'} {ticker || 'Stock'}
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
              {type === 'buy' ? 'Add to your portfolio' : 'Sell from your portfolio'}
            </Typography>
          </Box>

          {/* Content */}
          <Box sx={{ p: 3, backgroundColor: 'white' }}>
            <form onSubmit={handleSubmit}>
              <Stack spacing={3}>
                {!symbol && (
                  <TextField
                    label="Ticker Symbol"
                    value={ticker}
                    onChange={(e) => setTicker(e.target.value.toUpperCase())}
                    required
                    fullWidth
                    variant="outlined"
                    InputProps={{
                      startAdornment: <TrendingUpIcon sx={{ mr: 1, ml: 1, color: 'text.secondary' }} />,
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
                )}

                <TextField
                  label="Price per Share"
                  type="number"
                  value={price}
                  onChange={(e) => setPrice(e.target.value)}
                  required
                  fullWidth
                  variant="outlined"
                  InputProps={{
                    startAdornment: <AttachMoneyIcon sx={{ mr: 1, ml: 1, color: 'text.secondary' }} />,
                    inputProps: {
                      min: 0,
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

                <TextField
                  label="Quantity"
                  type="number"
                  value={quantity}
                  onChange={(e) => setQuantity(e.target.value)}
                  required
                  fullWidth
                  variant="outlined"
                  InputProps={{
                    startAdornment: <NumbersIcon sx={{ mr: 1, ml: 1, color: 'text.secondary' }} />,
                    inputProps: {
                      min: 1,
                      step: 1
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

                {/* Total Calculation */}
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
                      Total Value:
                    </Typography>
                    <Typography variant="h6" sx={{ fontWeight: 700, color: '#667eea' }}>
                      ${total}
                    </Typography>
                  </Box>
                </Paper>

                <Divider sx={{ my: 1 }} />

                {/* Action Buttons */}
                <Stack direction="row" spacing={2}>
                  <Button
                    variant="contained"
                    type="submit"
                    fullWidth
                    size="large"
                    sx={{
                      backgroundColor: type === 'buy' ? '#667eea' : '#d32f2f',
                      borderRadius: 2,
                      py: 1.5,
                      fontWeight: 600,
                      textTransform: 'none',
                      fontSize: '1rem',
                      '&:hover': {
                        backgroundColor: type === 'buy' ? '#5a6fd8' : '#c62828',
                        transform: 'translateY(-1px)',
                        boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
                      },
                      transition: 'all 0.2s ease-in-out',
                    }}
                  >
                    {type === 'buy' ? 'Buy Shares' : 'Sell Shares'}
                  </Button>
                  <Button
                    variant="outlined"
                    onClick={onClose}
                    fullWidth
                    size="large"
                    sx={{
                      borderRadius: 2,
                      py: 1.5,
                      fontWeight: 600,
                      textTransform: 'none',
                      fontSize: '1rem',
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