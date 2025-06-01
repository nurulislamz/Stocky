import { useState, useEffect } from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import CircularProgress from "@mui/material/CircularProgress";
import { useSearchParams } from 'react-router-dom';
import TVStockChartWidget from "../../components/tradingview/TVStockChartWidget";
import TVStockNewsWidget from "../../components/tradingview/TVStockNewsWidget";
import Grid from "@mui/material/Grid";

interface SearchResultsProps {
  symbol: string;
}

export default function SearchResults({ symbol }: SearchResultsProps) {
  const [searchParams] = useSearchParams();
  const [isLoading, setIsLoading] = useState(true);
  const [results, setResults] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);

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
      <Typography align="center" variant="h6">Search Results for: {symbol}</Typography>
      <Grid
        container
        spacing={2}
        columns={12}
        paddingTop={2}
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
          <TVStockChartWidget symbol={`${symbol}`} theme="light" interval="H" locale="en" autosize={true} />
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
          <TVStockNewsWidget feedMode="symbol" symbol={`${symbol}`} displayMode="compact" />
        </Grid>
      </Grid>
    </Box>
  );
}
