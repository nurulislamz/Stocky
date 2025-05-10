import {useState, useEffect} from 'react';
import FormControl from '@mui/material/FormControl';
import InputAdornment from '@mui/material/InputAdornment';
import OutlinedInput from '@mui/material/OutlinedInput';
import SearchRoundedIcon from '@mui/icons-material/SearchRounded';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Paper from '@mui/material/Paper';
import { useNavigate } from 'react-router-dom';

export default function SearchHero() {
  const navigate = useNavigate();
  const [search, setSearch] = useState<string>("");
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string>("");
  const [serverError, setServerError] = useState<string | null>(null);

  const handleSearch = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Enter' && search.trim()) {
      navigate(`/search?q=${encodeURIComponent(search.trim())}`);
    }
  };

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '60vh', // Takes up more vertical space
        width: '100%',
        maxWidth: '800px', // Wider than before
        mx: 'auto',
        px: 2
      }}
    >
      {/* Logo or Title */}
      <Typography
        component="h1"
        variant="h1"
        sx={{
          mb: 4,
          fontWeight: 'bold',
          textAlign: 'center',
          fontSize: { xs: '2.5rem', md: '3.5rem' }
        }}
      >
        Stocky
      </Typography>

      {/* Search Box */}
      <Paper
        elevation={3}
        sx={{
          width: '100%',
          maxWidth: '600px',
          borderRadius: '24px',
          overflow: 'hidden',
          '&:hover': {
            boxShadow: 6
          }
        }}
      >
        <FormControl
          fullWidth
          variant="outlined"
        >
          <OutlinedInput
            type="text"
            name="searchHero"
            autoFocus
            required
            fullWidth
            color={error ? "error" : "primary"}
            onChange={(e) => {
              setSearch(e.target.value);
              setServerError(null);
            }}
            size="medium"
            id="search-hero"
            placeholder="Search stocks..."
            sx={{
              height: '56px',
              fontSize: '1.1rem',
              '& fieldset': {
                border: 'none'
              },
              '&:hover fieldset': {
                border: 'none'
              },
              '&.Mui-focused fieldset': {
                border: 'none'
              }
            }}
            startAdornment={
              <InputAdornment position="start" sx={{ color: 'text.primary', pl: 2 }}>
                <SearchRoundedIcon fontSize="large" />
              </InputAdornment>
            }
            inputProps={{
              'aria-label': 'search',
            }}
            onKeyDown={handleSearch}
          />
        </FormControl>
      </Paper>

      {/* Optional: Quick Links or Suggestions */}
      <Box
        sx={{
          mt: 4,
          display: 'flex',
          gap: 2,
          flexWrap: 'wrap',
          justifyContent: 'center'
        }}
      >
        <Typography variant="body2" color="text.secondary">
          Popular:
        </Typography>
        <Typography variant="body2" color="primary" sx={{ cursor: 'pointer', '&:hover': { textDecoration: 'underline' } }}>
          AAPL
        </Typography>
        <Typography variant="body2" color="primary" sx={{ cursor: 'pointer', '&:hover': { textDecoration: 'underline' } }}>
          MSFT
        </Typography>
        <Typography variant="body2" color="primary" sx={{ cursor: 'pointer', '&:hover': { textDecoration: 'underline' } }}>
          GOOGL
        </Typography>
      </Box>
    </Box>
  );
}
