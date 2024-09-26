import { Box, Button, Center, Flex, Title } from '@mantine/core';
import { useNavigate } from "react-router-dom";

type HeaderProps = {
    title: string; // titleをpropsとして受け取る
};

export default function Header({title}:HeaderProps) {
    const navigate = useNavigate()
    return (
        <Box bg="#6F93C7" py="7" mb="xl">
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
                        size='md'
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