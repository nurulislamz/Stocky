import React from "react";
import Box from "@mui/material/Box";
import Link from "@mui/material/Link"

const SignUpFooter = () => {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: 2, alignItems:"center",  justifyContent:"center"}}>
      <Link href="#">Login</Link>
    </Box>
  )
}

export default SignUpFooter;