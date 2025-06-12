import * as React from 'react';
import Modal from '@mui/material/Modal';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';

interface TradeModalProps {
  open: boolean;
  onClose: () => void;
  symbol: string;
  price: string;
  type: 'buy' | 'sell';
}

export default function TradeModal({ open, onClose, symbol, price, type }: TradeModalProps) {
  const [quantity, setQuantity] = React.useState('');

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // TODO: Implement trade logic
    console.log(`${type.toUpperCase()} ${quantity} shares of ${symbol} at ${price}`);
    onClose();
  };

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
        width: 400,
        bgcolor: 'background.paper',
        boxShadow: 24,
        p: 4,
        borderRadius: 2,
      }}>
        <Typography id="trade-modal-title" variant="h6" component="h2" gutterBottom>
          {type === 'buy' ? 'Buy' : 'Sell'} {symbol}
        </Typography>
        <Typography variant="subtitle1" color="text.secondary" gutterBottom>
          Current Price: {price}
        </Typography>
        <form onSubmit={handleSubmit}>
          <Stack spacing={3}>
            <TextField
              label="Quantity"
              type="number"
              value={quantity}
              onChange={(e) => setQuantity(e.target.value)}
              required
              fullWidth
              inputProps={{ min: 1 }}
            />
            <Typography variant="body2" color="text.secondary">
              Total: ${quantity ? (parseFloat(price.replace('$', '')) * parseInt(quantity)).toFixed(2) : '0.00'}
            </Typography>
            <Stack direction="row" spacing={2}>
              <Button
                variant="contained"
                type="submit"
                fullWidth
                sx={{
                  backgroundColor: type === 'buy' ? '#1565c0' : '#d32f2f',
                  '&:hover': {
                    backgroundColor: type === 'buy' ? '#1565c0' : '#c62828'
                  }
                }}
              >
                {type === 'buy' ? 'Buy' : 'Sell'}
              </Button>
              <Button
                variant="outlined"
                onClick={onClose}
                fullWidth
              >
                Cancel
              </Button>
            </Stack>
          </Stack>
        </form>
      </Box>
    </Modal>
  );
}