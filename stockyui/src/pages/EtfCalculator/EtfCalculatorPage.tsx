import type {} from "@mui/x-charts/themeAugmentation";
import type {} from "@mui/x-data-grid-pro/themeAugmentation";
import type {} from "@mui/x-tree-view/themeAugmentation";
import Header from "../../components/Header";
import Layout from "../../templates/Layout";
import EtfCalculatorResult from "./EtfCalculatorResult";
import EtfCalculatorHero from "./EtfCalculatorHero";
import { useSearchParams } from "react-router-dom";

export default function EtfCaculatorPage() {
  const [searchParams] = useSearchParams();
  const query = searchParams.get("q") || '';

  return (
    <Layout>
      <Header headerName="Leverage Etf Calcualtor" />
      {query ? <EtfCalculatorResult symbol={query} /> : <EtfCalculatorHero />}
    </Layout>
  );
}