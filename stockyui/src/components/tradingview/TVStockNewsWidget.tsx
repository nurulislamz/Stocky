import React, { useEffect, useRef, memo } from 'react';

interface TVStockNewsWidgetProps {
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

function TVStockNewsWidget({
  feedMode,
  isTransparent = false,
  displayMode = "regular",
  width = "100%",
  height = "100%",
  colorTheme = "light",
  locale = "en",
  symbol = null,
  market = null
}: TVStockNewsWidgetProps) {
  const container = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const currentContainer = container.current;

    if (!currentContainer) {
      console.warn("Container is not available.");
      return;
    }

    // Only initialize once
      const script = document.createElement("script");
      script.src = "https://s3.tradingview.com/external-embedding/embed-widget-timeline.js";
      script.type = "text/javascript";
      script.async = true;

      const widgetConfig = {
        feedMode,
        isTransparent,
        displayMode,
        width,
        height,
        colorTheme,
        locale,
        symbol,
        market
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
  }, [feedMode, isTransparent, displayMode, width, height, colorTheme, locale, symbol, market]);

  return (
    <div
      className="tradingview-widget-container"
      ref={container}
      style={{ height: "100%", width: "100%" }}
    >
      <div
        className="tradingview-widget-container__widget"
        style={{ height: "100%", width: "100%" }}
      ></div>
    </div>
  );
}

export default memo(TVStockNewsWidget);