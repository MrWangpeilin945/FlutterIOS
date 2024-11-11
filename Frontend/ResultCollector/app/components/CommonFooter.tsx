import { Box, Button, Grid, GridCol } from "@mantine/core";
import { useNavigate } from "@remix-run/react";
import { createNavigate } from "~/utils/screenMove";

import styles from "~/styles/common.module.css";

type footerProps = {
  items?: {
    label: string;
    action: () => void;
  }[];
};

export default function CommonFooter({ items }: footerProps) {
  const navigate = useNavigate();
  const SC = createNavigate(navigate);
  let setItems = [{ label: "戻る", action: SC.navigateBack }];
  if (items) {
    if (items.length <= 3) {
      setItems = setItems.concat(items);
    }
  }

  return (
    <>
      {/*横並びにボタンを4つ配置*/}
      <Box h={60} className={styles.footer}>
        <Grid className={styles.footer}>
          {setItems?.map((item) => (
            <GridCol key={item.label} span={3} p={0}>
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
