import { useState } from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Paper from "@mui/material/Paper";
import Stack from "@mui/material/Stack";
import Chip from "@mui/material/Chip";
import TVStockChartWidget from "../../components/tradingview/TVStockChartWidget";
import Grid from "@mui/material/Grid";

interface EtfCalculatorResultProps {
  underlying: string;
  etf: string;
  leverage: string;
  price: string;
}

export default function EtfCalculatorResult({ underlying, etf, leverage, price }: EtfCalculatorResultProps) {
  const [error, setError] = useState<string | null>(null);

  if (error) {
    return (
      <Box sx={{ mt: 4 }}>
        <Typography color="error">{error}</Typography>
      </Box>
    );
  }

  const getLeverageLabel = (leverageValue: string) => {
    const value = parseInt(leverageValue);
    if (value < 0) return `${Math.abs(value)}x Inverse`;
    if (value === 1) return "1x Standard";
    return `${value}x Leveraged`;
  };

  const getLeverageColor = (leverageValue: string) => {
    const value = parseInt(leverageValue);
    if (value < 0) return "error";
    if (value === 1) return "default";
    return "primary";
  };

  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      {/* Header Section */}
      <Paper elevation={2} sx={{ p: 3, mb: 3, borderRadius: 2 }}>
        <Typography variant="h5" gutterBottom sx={{ fontWeight: 600 }}>
          ETF Performance Comparison
        </Typography>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mt: 2 }}>
          <Box>
            <Typography variant="body2" color="text.secondary">
              Underlying Asset
            </Typography>
            <Chip
              label={underlying}
              color="primary"
              variant="outlined"
              sx={{ fontWeight: 600, fontSize: '0.9rem' }}
            />
          </Box>

          <Box>
            <Typography variant="body2" color="text.secondary">
              Leveraged ETF
            </Typography>
            <Chip
              label={etf}
              color="secondary"
              variant="outlined"
              sx={{ fontWeight: 600, fontSize: '0.9rem' }}
            />
          </Box>

          <Box>
            <Typography variant="body2" color="text.secondary">
              Leverage
            </Typography>
            <Chip
              label={getLeverageLabel(leverage)}
              color={getLeverageColor(leverage) as any}
              sx={{ fontWeight: 600, fontSize: '0.9rem' }}
            />
          </Box>

          <Box>
            <Typography variant="body2" color="text.secondary">
              Price
            </Typography>
            <Chip
              label={`$${parseFloat(price).toFixed(2)}`}
              color="success"
              variant="outlined"
              sx={{ fontWeight: 600, fontSize: '0.9rem' }}
            />
          </Box>
        </Stack>
      </Paper>

      {/* Charts Section */}
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, lg: 6 }}>
          <Paper elevation={2} sx={{ p: 2, borderRadius: 2 }}>
            <Typography variant="h6" gutterBottom>
              {underlying} - Underlying Asset
            </Typography>
            <Box
              sx={{
                height: {
                  xs: "300px",
                  md: "400px",
                  lg: "450px",
                },
              }}
            >
              <TVStockChartWidget
                symbol={underlying}
                theme="light"
                interval="D"
                locale="en"
                autosize={true}
              />
            </Box>
          </Paper>
        </Grid>

        <Grid size={{ xs: 12, lg: 6 }}>
          <Paper elevation={2} sx={{ p: 2, borderRadius: 2 }}>
            <Typography variant="h6" gutterBottom>
              {etf} - Leveraged ETF ({getLeverageLabel(leverage)})
            </Typography>
            <Box
              sx={{
                height: {
                  xs: "300px",
                  md: "400px",
                  lg: "450px",
                },
              }}
            >
              <TVStockChartWidget
                symbol={etf}
                theme="light"
                interval="D"
                locale="en"
                autosize={true}
              />
            </Box>
          </Paper>
        </Grid>
      </Grid>

      {/* Performance Analysis Section */}
      <Paper elevation={2} sx={{ p: 3, mt: 3, borderRadius: 2 }}>
        <Typography variant="h6" gutterBottom>
          Performance Analysis
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Compare the price movements between {underlying} and {etf} to understand how the {getLeverageLabel(leverage)}
          affects returns. Leveraged ETFs typically amplify the daily movements of the underlying asset by the leverage factor.
        </Typography>

        {parseInt(leverage) < 0 && (
          <Box sx={{ mt: 2, p: 2, bgcolor: 'error.light', borderRadius: 1 }}>
            <Typography variant="body2" color="error.dark">
              <strong>Note:</strong> This is an inverse ETF that moves opposite to the underlying asset.
              When {underlying} goes up, {etf} typically goes down, and vice versa.
            </Typography>
          </Box>
        )}

        {parseInt(leverage) > 1 && (
          <Box sx={{ mt: 2, p: 2, bgcolor: 'warning.light', borderRadius: 1 }}>
            <Typography variant="body2" color="warning.dark">
              <strong>Warning:</strong> Leveraged ETFs are designed for short-term trading and may not track
              the underlying asset's long-term performance due to compounding effects.
            </Typography>
          </Box>
        )}
      </Paper>
    </Box>
  );
}
