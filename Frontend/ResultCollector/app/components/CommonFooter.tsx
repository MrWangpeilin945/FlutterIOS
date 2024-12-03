import { Box, Button, Grid, GridCol, Text } from "@mantine/core";
import { useNavigate } from "@remix-run/react";

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
      <Box>
        <Grid
          w="100%"
          pos="fixed"
          bottom={0}
          gutter={{ base: 4 }}
          bg="background"
        >
          {setItems?.map((item, index) => (
            <GridCol key={index} span={3}>
              <Button
                fullWidth
                h={59}
                variant="outline"
                bg="gray02"
                c="white01"
                color="gray02"
                radius={0}
                onClick={() => item.action?.()}
              >
                <Text size="lg" fw={700} c="white01" ta="center">
                  {item.label}
                </Text>
              </Button>
            </GridCol>
          ))}
        </Grid>
      </Box>
    </>
  );
}
