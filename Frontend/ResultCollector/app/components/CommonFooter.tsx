import { Box, Button, Grid, GridCol } from "@mantine/core";
import { useNavigate } from "@remix-run/react";
import styles from "~/styles/common.module.css";

type FooterProps = {
  items?: {
    label: string;
    action: () => void;
  }[];
};

export default function CommonFooter({ items }: FooterProps) {
  const navigate = useNavigate();
  let setItems = [{ label: "戻る", action: () => navigate(-1) }];
  const dummyItem = [{ label: "", action: () => {} }];

  if (items && items.length <= 3) {
    setItems = setItems.concat(items);
  }
  for (let i = setItems.length; i <= 3; i++) {
    setItems = setItems.concat(dummyItem);
  }

  return (
    <>
      {/*横並びにボタンを4つ配置*/}
      <Box h={60} className={styles.footer}>
        <Grid className={styles.footer}>
          {setItems?.map((item) => (
            <GridCol key={crypto.randomUUID()} span={3} p={0}>
              <Button
                className={styles["footer-button-text"]}
                fullWidth
                h={70}
                variant="outline"
                color="rgba(255, 255, 255, 1)"
                radius="0"
                onClick={() => item.action?.()}
              >
                {item.label}
              </Button>
            </GridCol>
          ))}
        </Grid>
      </Box>
    </>
  );
}
