import React from "react";
import Box from "@mui/material/Box";
import Link from "@mui/material/Link"

const LoginFooter = () => {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
      <Link href="#">Forgot password?</Link>
      <Link href="#">Sign up</Link>
    </Box>
  )
}

export default LoginFooter;