import { useEffect } from "react";
import { useNavigate } from "@remix-run/react";

export default function Index() {
  const navigate = useNavigate();

  // "/home"へリダイレクトする
  useEffect(() => {
    navigate("/home");
  }, []);

  return null;
}
