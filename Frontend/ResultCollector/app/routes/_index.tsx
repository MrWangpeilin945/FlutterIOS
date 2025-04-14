import { useEffect } from "react";
import { useNavigate } from "react-router";

export default function Index() {
  const navigate = useNavigate();

  // "/home"へリダイレクトする
  useEffect(() => {
    navigate("/home");
  }, []);

  return null;
}
