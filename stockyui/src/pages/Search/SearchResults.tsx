import { useState, useEffect } from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import CircularProgress from "@mui/material/CircularProgress";
import { useSearchParams } from 'react-router-dom';
import TVStockChartWidget from "../../components/tradingview/TVStockChartWidget";
import TVStockNewsWidget from "../../components/tradingview/TVStockNewsWidget";
import Grid from "@mui/material/Grid";
import Button from "@mui/material/Button";
import Stack from "@mui/material/Stack";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import Chip from "@mui/material/Chip";
import TradeModal from "../../components/TradeModal";

interface SearchResultsProps {
  symbol: string;
}

export default function SearchResults({ symbol }: SearchResultsProps) {
  const [searchParams] = useSearchParams();
  const [isLoading, setIsLoading] = useState(true);
  const [results, setResults] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [tradeType, setTradeType] = useState<'buy' | 'sell'>('buy');

  useEffect(() => {
    const fetchResults = async () => {
      if (!symbol) return;
      setIsLoading(true);
      try {
        // Fetch results
        // Update state
      } catch (error) {
        // Handle error
      } finally {
        setIsLoading(false);
      }
    };

    fetchResults();
  }, [symbol]);

  const handleTradeClick = (type: 'buy' | 'sell') => {
    setTradeType(type);
    setModalOpen(true);
  };

  const handleModalClose = () => {
    setModalOpen(false);
  };

  if (isLoading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", mt: 4 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ mt: 4 }}>
        <Typography color="error">{error}</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      <Card variant="outlined" sx={{ height: "100%", flexGrow: 1, mb: 2 }}>
        <CardContent>
          <Typography component="h2" variant="h4">
            {symbol.toUpperCase()}
          </Typography>
          <Stack
            direction="column"
            sx={{ justifyContent: "space-between", flexGrow: "1", gap: 1 }}
          >
            <Stack sx={{ justifyContent: "space-between" }}>
              <Stack
                direction="row"
                sx={{ justifyContent: "space-between", alignItems: "center" }}
              >
                <Typography variant="h4" component="p">
                  $100.00
                </Typography>
                <Chip size="small" color="success" label="+2.5%" />
              </Stack>
              <Typography variant="caption" sx={{ color: "text.secondary" }}>
                Company Name
              </Typography>
            </Stack>
            <Stack direction="row" spacing={2} sx={{ width: '100%', justifyContent: 'space-between' }}>
              <Button
                variant="contained"
                onClick={() => handleTradeClick('buy')}
                sx={{
                  flex: 1,
                  backgroundColor: '#1565c0 !important',
                  '&.MuiButton-contained': {
                    backgroundColor: '#1565c0 !important'
                  },
                  '&:hover': {
                    backgroundColor: '#1565c0 !important'
                  }
                }}
              >
                Buy
              </Button>
              <Button
                variant="contained"
                onClick={() => handleTradeClick('sell')}
                sx={{
                  backgroundColor: '#d32f2f',
                  flex: 1,
                  '&:hover': {
                    backgroundColor: '#c62828'
                  }
                }}
              >
                Sell
              </Button>
            </Stack>
          </Stack>
        </CardContent>
      </Card>

      <Grid
        container
        spacing={2}
        columns={12}
        sx={{ mb: (theme) => theme.spacing(2) }}
      >
        <Grid
          size={{ xs: 12, lg: 9 }}
          sx={{
            height: {
              xs: "300px",
              md: "400px",
              lg: "500px",
            },
          }}
        >
          <TVStockChartWidget
            symbol={`${symbol}`}
            theme="light"
            interval="H"
            locale="en"
            autosize={true}
          />
        </Grid>
        <Grid
          size={{ xs: 12, lg: 3 }}
          sx={{
            height: {
              xs: "300px",
              md: "400px",
              lg: "500px",
            },
          }}
        >
          <TVStockNewsWidget
            feedMode="symbol"
            symbol={`${symbol}`}
            displayMode="compact"
          />
        </Grid>
      </Grid>

      <TradeModal
        open={modalOpen}
        onClose={handleModalClose}
        symbol={symbol}
        price="$100.00"
        type={tradeType}
      />
    </Box>
  );
}
