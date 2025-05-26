import React, { useEffect, useRef, memo } from 'react';

interface TVStockChartWidgetProps {
  symbol: string;
  theme?: 'light' | 'dark';
  interval?: string;
  locale?: string;
  autosize?: boolean;
  width?: string | null;
  height?: string | null;
  allowSymbolChange?: boolean;
}

function TVStockChartWidget({
  symbol,
  theme = "light",
  interval = "H", // Default interval
  locale = "en", // Default locale
  autosize = true, // Default to container size
  width = null,
  height = null,
  allowSymbolChange = true,
}: TVStockChartWidgetProps) {
  const container = useRef<HTMLDivElement>(null);

  useEffect(() => {
    console.log("useEffect triggered");
    console.log("Dependencies:", { symbol, autosize, theme, interval, locale, allowSymbolChange });

    const currentContainer = container.current;

    if (!currentContainer) {
      console.warn("Container is not available.");
      return;
    }

    // Only initialize once
      const script = document.createElement("script");
      script.src = "https://s3.tradingview.com/external-embedding/embed-widget-advanced-chart.js";
      script.type = "text/javascript";
      script.async = true;

      const widgetConfig = {
        autosize,
        symbol,
        interval,
        timezone: "Etc/UTC",
        theme,
        style: "1",
        locale,
        allow_symbol_change: allowSymbolChange,
        support_host: "https://www.tradingview.com"
      };

      script.innerHTML = JSON.stringify(widgetConfig);

      // Append the script and handle errors
      const timeoutId = setTimeout(() => {
        try {
          currentContainer.appendChild(script);
        } catch (error) {
          console.error("Failed to load TradingView widget script:", error);
        }
      }, 100);

      return () => {
        clearTimeout(timeoutId);
        if (currentContainer) {
          while (currentContainer.firstChild) {
            currentContainer.removeChild(currentContainer.firstChild);
          }
        }
      };
  }, [symbol, autosize, theme, interval, locale, allowSymbolChange]);

  return (
    <div className="tradingview-widget-container" ref={container} style={{ height: "100%", width: "100%" }}>
      <div className="tradingview-widget-container__widget" style={{ height: "100%", width: "100%" }}></div>
    </div>
  );
}

export default memo(TVStockChartWidget);