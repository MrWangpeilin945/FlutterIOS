import { Box, Button, Center, Flex, Title } from '@mantine/core';
import { useNavigate } from "react-router-dom";
import styles from "~/styles/common.module.css";

type HeaderProps = {
    title: string; // titleをpropsとして受け取る
};

export default function Header({title}:HeaderProps) {
    const navigate = useNavigate();
    return (
        <Box className={styles["basic-blue"]} py="7">
            <header>
                <Flex
                    justify="space-between"
                    align="center"
                    px="md"
                >
                    <Box w={60} />

                    {/* 班選択 */}
                    <Center>
                        <Title
                            order={1}
                            c="white"
                            fw={500}
                        >
                            {title}
                        </Title>
                    </Center>

                    {/* ホームボタン */}
                    <Button
                        w={80}
                        h={40}
                        variant="fill"
                        bg="#396B9E"
                        onClick={() => navigate("/")}
                    >
                        ホーム
                    </Button>
                </Flex>
            </header>
        </Box>
    );
}