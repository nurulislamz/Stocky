import type {} from "@mui/x-charts/themeAugmentation";
import type {} from "@mui/x-data-grid-pro/themeAugmentation";
import type {} from "@mui/x-tree-view/themeAugmentation";
import Header from "../../components/Header";
import Layout from "../../templates/Layout";
import EtfCalculatorHero from "./EtfCalculatorHero";

export default function EtfCalculatorPage(props: {
  disableCustomTheme?: boolean;
}) {
  return (
    <Layout>
      <Header headerName = "ETF Calculator" />
      <EtfCalculatorHero />
    </Layout>
  );
}
