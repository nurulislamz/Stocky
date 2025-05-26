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
  theme =  "light",
  interval = "D", // Default interval
  locale = "en", // Default locale
  autosize = true, // Default to container size
  width = null,
  height = null,
  allowSymbolChange = true,
}: TVStockChartWidgetProps) {
  const container = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!container.current) {
      return;
    }

    while (container.current.firstChild) {
      container.current.removeChild(container.current.firstChild);
    }

    try {
      const script = document.createElement("script");
      script.src =
        "https://s3.tradingview.com/external-embedding/embed-widget-advanced-chart.js";
      script.type = "text/javascript";
      script.async = true;

      const widgetConfig = {
        autosize: autosize,
        width: autosize ? null : width,
        height: autosize ? null : height,
        symbol: symbol,
        interval: interval,
        timezone: "Etc/UTC",
        theme: theme,
        style: "1",
        locale: locale,
        allow_symbol_change: allowSymbolChange,
        support_host: "https://www.tradingview.com",
      };

      script.innerHTML = JSON.stringify(widgetConfig);

      container.current.appendChild(script);
    } catch (error) {
      console.error("Failed to load TradingView widget script:", error);
    }
    return () => {
      if (container.current) {
        while (container.current.firstChild) {
          container.current.removeChild(container.current.firstChild);
        }
      }
    };
  }, [
    symbol,
    theme,
    interval,
    locale,
    allowSymbolChange,
    autosize,
    width,
    height,
  ]);

  return (
    <div className="tradingview-widget-container" ref={container} style={{ height: "100%", width: "100%" }}>
      <div className="tradingview-widget-container__widget" style={{ height: "calc(100% - 32px)", width: "100%" }}></div>
      <div className="tradingview-widget-copyright"><a href="https://www.tradingview.com/" rel="noopener nofollow" target="_blank"><span className="blue-text">Track all markets on TradingView</span></a></div>
    </div>
  );
}

export default memo(TVStockChartWidget);