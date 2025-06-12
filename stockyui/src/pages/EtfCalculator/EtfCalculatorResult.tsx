import { useState } from "react";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { useSearchParams } from "react-router-dom";
import TVStockChartWidget from "../../components/tradingview/TVStockChartWidget";
import Grid from "@mui/material/Grid";
import Search from "../../components/Search";
import SelectContent from "../../components/SelectContent";

export default function EtfCalculatorResult({symbol} : {symbol: string | null}) {
  const [search, setSearch] = useState('');
  const [searchParams] = useSearchParams();
  const [error, setError] = useState<string | null>(null);

  if (error) {
    return (
      <Box sx={{ mt: 4 }}>
        <Typography color="error">{error}</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ width: "100%", maxWidth: { sm: "100%", md: "1700px" } }}>
      <Typography align="center" variant="h6">Leverage Etf Calculator for: {search}</Typography>

      <Grid
        container
        spacing={2}
        columns={12}
        paddingTop={2}
        sx={{ mb: (theme) => theme.spacing(2) }}
      >

      </Grid>

      <Grid
        container
        spacing={2}
        columns={12}
        paddingTop={2}
        sx={{ mb: (theme) => theme.spacing(2) }}
      >


        {symbol && (
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
          <TVStockChartWidget symbol={`${search}`} theme="light" interval="H" locale="en" autosize={true} />
        </Grid>
      )}
      </Grid>
    </Box>
  );
}
