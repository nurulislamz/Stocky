import { memo } from 'react';
import TVWidget from './TVWidget';

export interface TVStockNewsWidgetProps {
  feedMode: "all_symbols" | "symbol" | "market",
  isTransparent?: boolean,
  displayMode?: "regular" | "compact",
  width?: string,
  height?: string,
  colorTheme?: "light" | "dark",
  locale?: "en",
  symbol?: string | null,
  market?: string | null
}

const TVStockNewsWidget = (widgetProps : TVStockNewsWidgetProps) => {
  return (
    <TVWidget
      scriptSrc="https://s3.tradingview.com/external-embedding/embed-widget-timeline.js"
      widgetConfig={widgetProps}
    />
  );
}

export default memo(TVStockNewsWidget);