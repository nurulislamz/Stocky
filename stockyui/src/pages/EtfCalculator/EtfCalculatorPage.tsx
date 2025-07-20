import type {} from "@mui/x-charts/themeAugmentation";
import type {} from "@mui/x-data-grid-pro/themeAugmentation";
import type {} from "@mui/x-tree-view/themeAugmentation";
import Header from "../../components/Header";
import Layout from "../../templates/Layout";
import EtfCalculatorResult from "./EtfCalculatorResult";
import EtfCalculatorHero from "./EtfCalculatorHero";
import { useSearchParams } from "react-router-dom";

export default function EtfCalculatorPage() {
  const [searchParams] = useSearchParams();
  const underlying = searchParams.get("underlying");
  const etf = searchParams.get("etf");
  const leverage = searchParams.get("leverage");
  const price = searchParams.get("price");

  // Show results if we have the required parameters
  const hasRequiredParams = underlying && etf && leverage && price;

  return (
    <Layout>
      <Header headerName="Leverage ETF Calculator" />
      {hasRequiredParams ? (
        <EtfCalculatorResult
          underlying={underlying}
          etf={etf}
          leverage={leverage}
          price={price}
        />
      ) : (
        <EtfCalculatorHero />
      )}
    </Layout>
  );
}